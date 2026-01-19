namespace SSS.Quality1500.Presentation.Models;
using Microsoft.Extensions.Logging;


/// <summary>
/// Modelo para una entrada de log del sistema
/// Principio: Single Responsibility - Solo representa datos de log
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Timestamp cuando se generó el log
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Nivel del log (Debug, Info, Warning, Error, etc.)
    /// </summary>
    public LogLevel Level { get; init; }

    /// <summary>
    /// Categoría/fuente del log (ej: TSA.Dashboard._837P.Business.Services.Start837ProcessRefactored)
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Mensaje principal del log
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Excepción asociada al log (si existe)
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Información adicional del scope/contexto
    /// </summary>
    public string? Scope { get; init; }

    /// <summary>
    /// ID único para la entrada de log
    /// </summary>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Constructor interno para uso exclusivo del LogEntryBuilder
    /// Se hizo interno para forzar el uso del patrón Builder y cumplir con la regla de máximo 3 parámetros
    /// </summary>
    internal LogEntry(DateTime timestamp, LogLevel level, string category, string message, Exception? exception, string? scope)
    {
        Timestamp = timestamp;
        Level = level;
        Category = category;
        Message = message;
        Exception = exception;
        Scope = scope;
    }

    /// <summary>
    /// Obtiene el texto completo del log incluyendo excepción si existe
    /// </summary>
    public string GetFullText()
    {
        var text = $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level}] {Category}: {Message}";

        if (!string.IsNullOrEmpty(Scope))
        {
            text += $" (Scope: {Scope})";
        }

        if (Exception != null)
        {
            text += $"\nException: {Exception}";
        }

        return text;
    }

    /// <summary>
    /// Obtiene el color sugerido para mostrar el log en UI
    /// </summary>
    public string GetDisplayColor()
    {
        return Level switch
        {
            LogLevel.Critical => "#FF8B0000", // DarkRed
            LogLevel.Error => "#FFDC143C",    // Crimson
            LogLevel.Warning => "#FFFF8C00",  // DarkOrange
            LogLevel.Information => "#FF0000FF", // Blue
            LogLevel.Debug => "#FF808080",    // Gray
            LogLevel.Trace => "#FFA9A9A9",    // LightGray
            _ => "#FF000000"                  // Black
        };
    }

    /// <summary>
    /// Indica si este log representa un error crítico
    /// </summary>
    public bool IsError => Level == LogLevel.Error || Level == LogLevel.Critical;

    /// <summary>
    /// Indica si este log es de alta prioridad (Warning o superior)
    /// </summary>
    public bool IsHighPriority => Level >= LogLevel.Warning;

    /// <summary>
    /// Crea un nuevo builder para LogEntry
    /// Método de conveniencia para usar el patrón Builder
    /// </summary>
    /// <returns>Nueva instancia del builder</returns>
    public static LogEntryBuilder Builder() => LogEntryBuilder.Create();

    /// <summary>
    /// Crea un nuevo builder con valores básicos pre-configurados
    /// Método de conveniencia para usar el patrón Builder
    /// </summary>
    /// <param name="level">Nivel del log</param>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder con valores básicos configurados</returns>
    public static LogEntryBuilder Builder(LogLevel level, string category, string message) =>
        LogEntryBuilder.Create(level, category, message);

    /// <summary>
    /// Crea un log de información usando el builder
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para información</returns>
    public static LogEntryBuilder Information(string category, string message) =>
        LogEntryBuilder.Information(category, message);

    /// <summary>
    /// Crea un log de error usando el builder
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para error</returns>
    public static LogEntryBuilder Error(string category, string message) =>
        LogEntryBuilder.Error(category, message);

    /// <summary>
    /// Crea un log de error con excepción usando el builder
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <param name="exception">Excepción del log</param>
    /// <returns>Builder configurado para error con excepción</returns>
    public static LogEntryBuilder Error(string category, string message, Exception exception) =>
        LogEntryBuilder.Error(category, message, exception);

    /// <summary>
    /// Crea un log de warning usando el builder
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para warning</returns>
    public static LogEntryBuilder Warning(string category, string message) =>
        LogEntryBuilder.Warning(category, message);

    /// <summary>
    /// Crea un log de debug usando el builder
    /// </summary>
    /// <param name="category">Categoría del log</param>
    /// <param name="message">Mensaje del log</param>
    /// <returns>Builder configurado para debug</returns>
    public static LogEntryBuilder Debug(string category, string message) =>
        LogEntryBuilder.Debug(category, message);
}
