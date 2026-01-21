namespace SSS.Quality1500.Presentation.Services;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Presentation.Models;


/// <summary>
/// Servicio para parsear archivos de log de Serilog y convertirlos a LogEntry
/// Principio: Single Responsibility - Solo se encarga de parsear archivos de log
/// Principio: Open/Closed - Extensible para diferentes formatos de log
/// </summary>
public static class LogFileParser
{
    // Patrón regex para el formato de Serilog: 2025-07-07 21:35:21.123 [INF] Message
    private static readonly Regex LogLineRegex = new(
        @"^(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3})\s+\[(?<level>\w{3})\]\s+(?<message>.*)$",
        RegexOptions.Compiled | RegexOptions.Multiline);

    // Mapeo de niveles de Serilog a LogLevel de Microsoft.Extensions.Logging
    private static readonly Dictionary<string, LogLevel> LogLevelMapping = new()
    {
        { "TRC", LogLevel.Trace },
        { "DBG", LogLevel.Debug },
        { "INF", LogLevel.Information },
        { "WRN", LogLevel.Warning },
        { "ERR", LogLevel.Error },
        { "FTL", LogLevel.Critical }
    };

    /// <summary>
    /// Parsea un archivo de log completo y devuelve las entradas de log
    /// </summary>
    /// <param name="filePath">Ruta del archivo de log</param>
    /// <param name="lastPosition">Posición desde donde empezar a leer (para actualizaciones incrementales)</param>
    /// <returns>Lista de LogEntry parseadas y nueva posición del archivo</returns>
    public static async Task<(IEnumerable<LogEntry> entries, long newPosition)> ParseLogFileAsync(
        string filePath,
        long lastPosition = 0)
    {
        if (!File.Exists(filePath))
            return (Enumerable.Empty<LogEntry>(), 0);

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var fileInfo = new FileInfo(filePath);

            // Si el archivo es más pequeño que la última posición, significa que fue rotado
            if (fileInfo.Length < lastPosition)
                lastPosition = 0;

            fileStream.Seek(lastPosition, SeekOrigin.Begin);

            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();
            var newPosition = fileStream.Position;

            if (string.IsNullOrWhiteSpace(content))
                return (Enumerable.Empty<LogEntry>(), newPosition);

            var entries = ParseLogContent(content, Path.GetFileName(filePath));
            return (entries, newPosition);
        }
        catch (Exception ex)
        {
            // En caso de error, retornar una entrada de log con el error usando Builder Pattern
            var errorEntry = LogEntry.Error("LogFileParser", $"Error parsing log file {filePath}: {ex.Message}", ex)
                .WithTimestamp(DateTime.Now)
                .Build();

            return (new[] { errorEntry }, lastPosition);
        }
    }

    /// <summary>
    /// Parsea el contenido de texto de un log y devuelve las entradas
    /// </summary>
    /// <param name="content">Contenido del log</param>
    /// <param name="fileName">Nombre del archivo (para categoría)</param>
    /// <returns>Lista de LogEntry parseadas</returns>
    public static IEnumerable<LogEntry> ParseLogContent(string content, string fileName)
    {
        if (string.IsNullOrWhiteSpace(content))
            yield break;

        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        LogEntry? currentEntry = null;
        var multiLineMessage = new List<string>();

        foreach (var line in lines)
        {
            var trimmedLine = line.TrimEnd('\r');

            var match = LogLineRegex.Match(trimmedLine);

            if (match.Success)
            {
                // Si hay una entrada anterior, completarla y devolverla
                if (currentEntry != null)
                {
                    var completedEntry = CreateLogEntryWithMultilineMessage(currentEntry, multiLineMessage, fileName);
                    if (completedEntry != null)
                        yield return completedEntry;

                    multiLineMessage.Clear();
                }

                // Crear nueva entrada
                currentEntry = CreateLogEntryFromMatch(match, fileName);
            }
            else if (currentEntry != null)
            {
                // Esta línea es parte de un mensaje multilínea o excepción
                multiLineMessage.Add(trimmedLine);
            }
        }

        // Procesar la última entrada si existe
        if (currentEntry != null)
        {
            var finalEntry = CreateLogEntryWithMultilineMessage(currentEntry, multiLineMessage, fileName);
            if (finalEntry != null)
                yield return finalEntry;
        }
    }

    /// <summary>
    /// Crea una LogEntry desde un match de regex
    /// </summary>
    private static LogEntry? CreateLogEntryFromMatch(Match match, string fileName)
    {
        try
        {
            var timestampStr = match.Groups["timestamp"].Value;
            var levelStr = match.Groups["level"].Value;
            var message = match.Groups["message"].Value;

            if (!DateTime.TryParseExact(timestampStr, "yyyy-MM-dd HH:mm:ss.fff",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var timestamp))
            {
                return null;
            }

            if (!LogLevelMapping.TryGetValue(levelStr, out var logLevel))
            {
                logLevel = LogLevel.Information; // Default si no se reconoce el nivel
            }

            var entry = LogEntry.Builder(logLevel, ExtractCategoryFromMessage(message), message)
                .WithTimestamp(timestamp)
                .Build();

            return entry;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Completa una LogEntry con mensaje multilínea
    /// </summary>
    private static LogEntry? CreateLogEntryWithMultilineMessage(LogEntry baseEntry, List<string> additionalLines, string fileName)
    {
        if (baseEntry == null)
            return null;

        var fullMessage = baseEntry.Message;
        Exception? exception = null;

        if (additionalLines.Count > 0)
        {
            var additionalText = string.Join("\n", additionalLines);
            fullMessage += "\n" + additionalText;

            // Si las líneas adicionales parecen una excepción, tratarlas como tal
            if (additionalLines.Any(line => line.Contains("Exception") || line.Contains("at ") || line.Contains("in ")))
            {
                try
                {
                    exception = new Exception(additionalText);
                }
                catch
                {
                    // Si no se puede crear la excepción, mantener como texto
                }
            }
        }

        return LogEntry.Builder(baseEntry.Level, baseEntry.Category, fullMessage)
            .WithTimestamp(baseEntry.Timestamp)
            .WithException(exception ?? baseEntry.Exception)
            .Build();
    }

    /// <summary>
    /// Extrae la categoría del mensaje de log (normalmente es la primera parte antes de los dos puntos)
    /// </summary>
    private static string ExtractCategoryFromMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "Unknown";

        // Buscar patrones comunes de categoría en mensajes de Serilog
        var colonIndex = message.IndexOf(':');
        if (colonIndex > 0 && colonIndex < 100) // Límite razonable para categoría
        {
            var potentialCategory = message.Substring(0, colonIndex).Trim();

            // Verificar si parece una categoría válida (contiene namespace patterns)
            if (potentialCategory.Contains('.') && !potentialCategory.Contains(' '))
            {
                return potentialCategory;
            }
        }

        return "Application";
    }

    /// <summary>
    /// Obtiene todos los archivos de log en un directorio ordenados por fecha
    /// </summary>
    /// <param name="logDirectory">Directorio de logs</param>
    /// <returns>Lista de archivos de log ordenados</returns>
    public static IEnumerable<string> GetLogFiles(string logDirectory)
    {
        if (!Directory.Exists(logDirectory))
            return Enumerable.Empty<string>();

        try
        {
            return Directory.GetFiles(logDirectory, "*.log")
                .OrderBy(f => new FileInfo(f).CreationTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LogFileParser] Error obteniendo archivos de log en: {logDirectory} - {ex.Message}");
            return Enumerable.Empty<string>();
        }
    }
}
