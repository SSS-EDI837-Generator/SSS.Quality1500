namespace SSS.Quality1500.Presentation.Interfaces;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Presentation.Models;
using System.Collections.ObjectModel;


/// <summary>
/// Interfaz para el servicio de captura y gestión de logs
/// Principio: Interface Segregation - Solo expone las operaciones necesarias para logs
/// Principio: Dependency Inversion - Permite diferentes implementaciones
/// </summary>
public interface ILogCaptureService : IDisposable
{
    /// <summary>
    /// Colección observable de entradas de log para binding con UI
    /// </summary>
    ObservableCollection<LogEntry> LogEntries { get; }

    /// <summary>
    /// Evento que se dispara cuando se agrega una nueva entrada de log
    /// </summary>
    event EventHandler<LogEntry>? LogEntryAdded;

    /// <summary>
    /// Agregar una nueva entrada de log
    /// </summary>
    /// <param name="logEntry">La entrada de log a agregar</param>
    void AddLogEntry(LogEntry logEntry);

    /// <summary>
    /// Obtener los logs filtrados por nivel mínimo
    /// </summary>
    /// <param name="minLevel">Nivel mínimo de log a incluir</param>
    /// <returns>Lista de logs filtrados</returns>
    IEnumerable<LogEntry> GetLogsByLevel(LogLevel minLevel);

    /// <summary>
    /// Obtener los logs filtrados por categoría
    /// </summary>
    /// <param name="category">Categoría a filtrar (puede ser parcial)</param>
    /// <returns>Lista de logs filtrados</returns>
    IEnumerable<LogEntry> GetLogsByCategory(string category);

    /// <summary>
    /// Obtener los logs en un rango de fechas
    /// </summary>
    /// <param name="fromDate">Fecha inicial</param>
    /// <param name="toDate">Fecha final</param>
    /// <returns>Lista de logs en el rango</returns>
    IEnumerable<LogEntry> GetLogsByDateRange(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Obtener solo los logs de error
    /// </summary>
    /// <returns>Lista de logs de error</returns>
    IEnumerable<LogEntry> GetErrorLogs();

    /// <summary>
    /// Limpiar todos los logs
    /// </summary>
    void ClearLogs();

    /// <summary>
    /// Exportar logs a un archivo de texto
    /// </summary>
    /// <param name="filePath">Ruta del archivo donde exportar</param>
    /// <param name="logs">Logs a exportar (si es null, exporta todos)</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task ExportLogsToFileAsync(string filePath, IEnumerable<LogEntry>? logs = null);

    /// <summary>
    /// Obtener estadísticas de los logs
    /// </summary>
    /// <returns>Diccionario con estadísticas por nivel de log</returns>
    Dictionary<LogLevel, int> GetLogStatistics();

    /// <summary>
    /// Establecer el límite máximo de logs en memoria
    /// </summary>
    /// <param name="maxEntries">Número máximo de entradas</param>
    void SetMaxLogEntries(int maxEntries);

    /// <summary>
    /// Cargar logs desde archivos físicos
    /// </summary>
    /// <param name="logDirectory">Directorio donde están los archivos de log</param>
    /// <param name="forceReload">Si es true, limpia los logs existentes y recarga todos</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    Task LoadLogsFromFilesAsync(string? logDirectory = null, bool forceReload = false);

    /// <summary>
    /// Iniciar el monitoreo de archivos de log para cambios en tiempo real
    /// </summary>
    /// <param name="logDirectory">Directorio a monitorear</param>
    void StartFileWatcher(string? logDirectory = null);

    /// <summary>
    /// Detener el monitoreo de archivos de log
    /// </summary>
    void StopFileWatcher();

    /// <summary>
    /// Indica si el monitoreo de archivos está activo
    /// </summary>
    bool IsFileWatcherActive { get; }
}

