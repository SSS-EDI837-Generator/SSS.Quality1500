namespace SSS.Quality1500.Presentation.Services;

using Microsoft.Extensions.Logging;
using SSS.Quality1500.Presentation.Interfaces;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using SSS.Quality1500.Presentation.Models;

/// <summary>
/// Implementación del servicio de captura y gestión de logs
/// Principio: Single Responsibility - Solo maneja la captura y gestión de logs
/// Principio: Open/Closed - Extensible para nuevas funcionalidades de log
/// </summary>
public class LogCaptureService : ILogCaptureService
{
    private readonly ObservableCollection<LogEntry> _logEntries;
    private readonly ConcurrentQueue<LogEntry> _logQueue;
    private readonly object _lockObject = new object();
    private int _maxLogEntries = 1000; // Límite por defecto

    // Nuevos campos para monitoreo de archivos
    private FileSystemWatcher? _fileWatcher;
    private readonly Dictionary<string, long> _filePositions = new();
    private string? _logDirectory;

    /// <summary>
    /// Colección observable de entradas de log thread-safe
    /// </summary>
    public ObservableCollection<LogEntry> LogEntries => _logEntries;

    /// <summary>
    /// Evento disparado cuando se agrega una nueva entrada de log
    /// </summary>
    public event EventHandler<LogEntry>? LogEntryAdded;

    /// <summary>
    /// Indica si el monitoreo de archivos está activo
    /// </summary>
    public bool IsFileWatcherActive => _fileWatcher?.EnableRaisingEvents == true;

    /// <summary>
    /// Constructor del servicio de captura de logs
    /// </summary>
    public LogCaptureService()
    {
        _logEntries = new ObservableCollection<LogEntry>();
        _logQueue = new ConcurrentQueue<LogEntry>();

        // Configurar directorio de logs por defecto
        _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    }

    /// <summary>
    /// Agregar una nueva entrada de log de forma thread-safe
    /// </summary>
    /// <param name="logEntry">La entrada de log a agregar</param>
    public void AddLogEntry(LogEntry logEntry)
    {
        if (logEntry == null) return;

        _logQueue.Enqueue(logEntry);

        // Procesar en el hilo de UI para mantener thread-safety
        Application.Current?.Dispatcher.Invoke(() =>
        {
            lock (_lockObject)
            {
                // Agregar el nuevo log
                _logEntries.Add(logEntry);

                // Mantener el límite de logs en memoria
                while (_logEntries.Count > _maxLogEntries)
                {
                    _logEntries.RemoveAt(0);
                }
            }

            // Disparar evento
            LogEntryAdded?.Invoke(this, logEntry);
        });
    }

    /// <summary>
    /// Obtener logs filtrados por nivel mínimo
    /// </summary>
    /// <param name="minLevel">Nivel mínimo de log a incluir</param>
    /// <returns>Lista de logs filtrados</returns>
    public IEnumerable<LogEntry> GetLogsByLevel(LogLevel minLevel)
    {
        lock (_lockObject)
        {
            return _logEntries.Where(log => log.Level >= minLevel).OrderByDescending(log => log.Timestamp).ToList();
        }
    }

    /// <summary>
    /// Obtener logs filtrados por categoría
    /// </summary>
    /// <param name="category">Categoría a filtrar (búsqueda parcial)</param>
    /// <returns>Lista de logs filtrados</returns>
    public IEnumerable<LogEntry> GetLogsByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return Enumerable.Empty<LogEntry>();

        lock (_lockObject)
        {
            return _logEntries
                .Where(log => log.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(log => log.Timestamp)
                .ToList();
        }
    }

    /// <summary>
    /// Obtener logs en un rango de fechas
    /// </summary>
    /// <param name="fromDate">Fecha inicial</param>
    /// <param name="toDate">Fecha final</param>
    /// <returns>Lista de logs en el rango</returns>
    public IEnumerable<LogEntry> GetLogsByDateRange(DateTime fromDate, DateTime toDate)
    {
        lock (_lockObject)
        {
            return _logEntries
                .Where(log => log.Timestamp >= fromDate && log.Timestamp <= toDate)
                .OrderByDescending(log => log.Timestamp)
                .ToList();
        }
    }

    /// <summary>
    /// Obtener solo logs de error (Error y Critical)
    /// </summary>
    /// <returns>Lista de logs de error</returns>
    public IEnumerable<LogEntry> GetErrorLogs()
    {
        lock (_lockObject)
        {
            return _logEntries
                .Where(log => log.IsError)
                .OrderByDescending(log => log.Timestamp)
                .ToList();
        }
    }

    /// <summary>
    /// Limpiar todos los logs
    /// </summary>
    public void ClearLogs()
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            lock (_lockObject)
            {
                _logEntries.Clear();
            }
        });

        // Limpiar también la cola
        while (_logQueue.TryDequeue(out _)) { }
    }

    /// <summary>
    /// Exportar logs a archivo de texto
    /// </summary>
    /// <param name="filePath">Ruta del archivo</param>
    /// <param name="logs">Logs a exportar (null = todos)</param>
    /// <returns>Task de la operación</returns>
    public async Task ExportLogsToFileAsync(string filePath, IEnumerable<LogEntry>? logs = null)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("La ruta del archivo no puede estar vacía", nameof(filePath));

        var logsToExport = logs ?? GetAllLogs();

        var lines = logsToExport
            .OrderBy(log => log.Timestamp)
            .Select(log => log.GetFullText());

        await File.WriteAllLinesAsync(filePath, lines);
    }

    /// <summary>
    /// Obtener estadísticas de logs por nivel
    /// </summary>
    /// <returns>Diccionario con conteos por nivel</returns>
    public Dictionary<LogLevel, int> GetLogStatistics()
    {
        lock (_lockObject)
        {
            return _logEntries
                .GroupBy(log => log.Level)
                .ToDictionary(group => group.Key, group => group.Count());
        }
    }

    /// <summary>
    /// Establecer el límite máximo de logs en memoria
    /// </summary>
    /// <param name="maxEntries">Número máximo de entradas</param>
    public void SetMaxLogEntries(int maxEntries)
    {
        if (maxEntries <= 0)
            throw new ArgumentException("El número máximo de entradas debe ser mayor a 0", nameof(maxEntries));

        _maxLogEntries = maxEntries;

        // Aplicar el límite inmediatamente si es necesario
        Application.Current?.Dispatcher.Invoke(() =>
        {
            lock (_lockObject)
            {
                while (_logEntries.Count > _maxLogEntries)
                {
                    _logEntries.RemoveAt(0);
                }
            }
        });
    }

    /// <summary>
    /// Obtener todos los logs (método helper privado)
    /// </summary>
    /// <returns>Lista de todos los logs</returns>
    private IEnumerable<LogEntry> GetAllLogs()
    {
        lock (_lockObject)
        {
            return _logEntries.OrderByDescending(log => log.Timestamp).ToList();
        }
    }

    #region File Reading and Watching

    /// <summary>
    /// Cargar logs desde archivos físicos
    /// </summary>
    /// <param name="logDirectory">Directorio donde están los archivos de log</param>
    /// <param name="forceReload">Si es true, limpia los logs existentes y recarga todos</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    public async Task LoadLogsFromFilesAsync(string? logDirectory = null, bool forceReload = false)
    {
        var targetDirectory = logDirectory ?? _logDirectory;
        if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
        {
            return;
        }

        try
        {

            // Si es force reload, limpiar logs existentes y resetear posiciones
            if (forceReload)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    lock (_lockObject)
                    {
                        _logEntries.Clear();
                    }
                });
                _filePositions.Clear();
            }

            var logFiles = LogFileParser.GetLogFiles(targetDirectory).ToList();
            var totalEntriesLoaded = 0;


            foreach (var filePath in logFiles)
            {
                try
                {
                    var (entries, newPosition) = await LogFileParser.ParseLogFileAsync(filePath);

                    // Guardar la posición del archivo para futuros updates incrementales
                    _filePositions[filePath] = newPosition;

                    var entriesList = entries.ToList();

                    if (entriesList.Any())
                    {
                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            lock (_lockObject)
                            {
                                foreach (var entry in entriesList)
                                {
                                    // Si es forceReload, agregar sin verificar duplicados
                                    if (forceReload)
                                    {
                                        _logEntries.Add(entry);
                                        totalEntriesLoaded++;
                                    }
                                    else if (!_logEntries.Any(existing =>
                                        existing.Timestamp == entry.Timestamp &&
                                        existing.Message == entry.Message))
                                    {
                                        _logEntries.Add(entry);
                                        totalEntriesLoaded++;
                                    }
                                }

                                // Aplicar límite de logs
                                while (_logEntries.Count > _maxLogEntries)
                                {
                                    _logEntries.RemoveAt(0);
                                }

                                // Ordenar por timestamp
                                var sortedEntries = _logEntries.OrderBy(log => log.Timestamp).ToList();
                                _logEntries.Clear();
                                foreach (var entry in sortedEntries)
                                {
                                    _logEntries.Add(entry);
                                }
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LogCaptureService] Error procesando archivo de log: {filePath} - {ex.Message}");
                }
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LogCaptureService] Error cargando logs desde archivos en: {targetDirectory} - {ex.Message}");
        }
    }

    /// <summary>
    /// Iniciar el monitoreo de archivos de log para cambios en tiempo real
    /// </summary>
    /// <param name="logDirectory">Directorio a monitorear</param>
    public void StartFileWatcher(string? logDirectory = null)
    {
        var targetDirectory = logDirectory ?? _logDirectory;
        if (string.IsNullOrWhiteSpace(targetDirectory) || !Directory.Exists(targetDirectory))
        {
            return;
        }

        try
        {
            StopFileWatcher(); // Detener watcher anterior si existe

            _fileWatcher = new FileSystemWatcher(targetDirectory)
            {
                Filter = "*.log",
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.CreationTime,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            _fileWatcher.Changed += OnLogFileChanged;
            _fileWatcher.Created += OnLogFileCreated;
            _fileWatcher.Error += OnFileWatcherError;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LogCaptureService] Error iniciando el monitoreo de archivos de log en: {targetDirectory} - {ex.Message}");
        }
    }

    /// <summary>
    /// Detener el monitoreo de archivos de log
    /// </summary>
    public void StopFileWatcher()
    {
        if (_fileWatcher != null)
        {
            try
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Changed -= OnLogFileChanged;
                _fileWatcher.Created -= OnLogFileCreated;
                _fileWatcher.Error -= OnFileWatcherError;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogCaptureService] Error deteniendo el monitoreo de archivos de log: {ex.Message}");
            }
        }
    }

    #endregion

    #region File Watcher Event Handlers

    /// <summary>
    /// Maneja cambios en archivos de log existentes
    /// </summary>
    private async void OnLogFileChanged(object sender, FileSystemEventArgs e)
    {
        await ProcessLogFileChange(e.FullPath);
    }

    /// <summary>
    /// Maneja la creación de nuevos archivos de log
    /// </summary>
    private async void OnLogFileCreated(object sender, FileSystemEventArgs e)
    {
        // Esperar un poco para que el archivo sea completamente creado
        await Task.Delay(500);
        await ProcessLogFileChange(e.FullPath);
    }

    /// <summary>
    /// Maneja errores del FileSystemWatcher
    /// </summary>
    private void OnFileWatcherError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine($"[LogCaptureService] Error en el monitoreo de archivos de log: {e.GetException()?.Message}");

        // Intentar reiniciar el watcher
        Task.Run(async () =>
        {
            await Task.Delay(5000); // Esperar 5 segundos
            try
            {
                StartFileWatcher(_logDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LogCaptureService] Error reiniciando el monitoreo de archivos de log: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// Procesa cambios en un archivo de log específico
    /// </summary>
    private async Task ProcessLogFileChange(string filePath)
    {
        try
        {
            // Obtener la última posición conocida del archivo
            var lastPosition = _filePositions.GetValueOrDefault(filePath, 0);

            var (entries, newPosition) = await LogFileParser.ParseLogFileAsync(filePath, lastPosition);

            // Actualizar la posición del archivo
            _filePositions[filePath] = newPosition;

            var entriesList = entries.ToList();
            if (entriesList.Any())
            {
                foreach (var entry in entriesList)
                {
                    AddLogEntry(entry);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LogCaptureService] Error procesando cambios en archivo de log: {filePath} - {ex.Message}");
        }
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Liberar recursos
    /// </summary>
    public void Dispose()
    {
        StopFileWatcher();
        _filePositions.Clear();
    }

    #endregion
}
