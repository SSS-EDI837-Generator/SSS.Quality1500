namespace SSS.Quality1500.Presentation.Services;

using Microsoft.Extensions.Logging;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Presentation.Models;

/// <summary>
/// Logger Provider personalizado que captura todos los logs de la aplicación
/// Principio: Single Responsibility - Solo se encarga de proveer loggers que capturen logs
/// Principio: Open/Closed - Extensible para diferentes tipos de captura
/// </summary>
public class CaptureLoggerProvider : ILoggerProvider
{
    private readonly ILogCaptureService _logCaptureService;
    private readonly LogLevel _minLogLevel;

    /// <summary>
    /// Constructor del provider
    /// </summary>
    /// <param name="logCaptureService">Servicio de captura de logs</param>
    /// <param name="minLogLevel">Nivel mínimo de logs a capturar</param>
    public CaptureLoggerProvider(ILogCaptureService logCaptureService, LogLevel minLogLevel = LogLevel.Debug)
    {
        _logCaptureService = logCaptureService ?? throw new ArgumentNullException(nameof(logCaptureService));
        _minLogLevel = minLogLevel;
    }

    /// <summary>
    /// Crear un logger para una categoría específica
    /// </summary>
    /// <param name="categoryName">Nombre de la categoría (ej: namespace.clase)</param>
    /// <returns>Logger personalizado</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new CaptureLogger(categoryName, _logCaptureService, _minLogLevel);
    }

    /// <summary>
    /// Dispose del provider
    /// </summary>
    public void Dispose()
    {
        // No hay recursos no administrados que liberar
    }
}

/// <summary>
/// Logger personalizado que captura logs y los envía al servicio de captura
/// Principio: Single Responsibility - Solo se encarga de capturar y enviar logs
/// </summary>
internal class CaptureLogger : ILogger
{
    private readonly string _categoryName;
    private readonly ILogCaptureService _logCaptureService;
    private readonly LogLevel _minLogLevel;

    /// <summary>
    /// Constructor del logger
    /// </summary>
    /// <param name="categoryName">Nombre de la categoría</param>
    /// <param name="logCaptureService">Servicio de captura</param>
    /// <param name="minLogLevel">Nivel mínimo a capturar</param>
    public CaptureLogger(string categoryName, ILogCaptureService logCaptureService, LogLevel minLogLevel)
    {
        _categoryName = categoryName;
        _logCaptureService = logCaptureService;
        _minLogLevel = minLogLevel;
    }

    /// <summary>
    /// Iniciar un scope de logging
    /// </summary>
    /// <typeparam name="TState">Tipo del estado</typeparam>
    /// <param name="state">Estado del scope</param>
    /// <returns>Disposable del scope</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null; // No implementamos scopes por simplicidad
    }

    /// <summary>
    /// Verificar si un nivel de log está habilitado
    /// </summary>
    /// <param name="logLevel">Nivel de log a verificar</param>
    /// <returns>True si está habilitado</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minLogLevel;
    }

    /// <summary>
    /// Método principal de logging que captura los logs
    /// </summary>
    /// <typeparam name="TState">Tipo del estado</typeparam>
    /// <param name="logLevel">Nivel del log</param>
    /// <param name="eventId">ID del evento</param>
    /// <param name="state">Estado/mensaje del log</param>
    /// <param name="exception">Excepción asociada</param>
    /// <param name="formatter">Formateador del mensaje</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);

        if (string.IsNullOrEmpty(message) && exception == null)
            return;

        // Crear la entrada de log usando Builder Pattern
        var logEntry = LogEntry.Builder(logLevel, _categoryName, message)
            .WithTimestamp(DateTime.Now)
            .WithException(exception)
            .WithScope(GetCurrentScope())
            .Build();

        // Enviar al servicio de captura
        _logCaptureService.AddLogEntry(logEntry);
    }

    /// <summary>
    /// Obtener información del scope actual (simplificado)
    /// </summary>
    /// <returns>Información del scope o null</returns>
    private string? GetCurrentScope()
    {
        // Por simplicidad, no implementamos scopes complejos
        // En una implementación más avanzada, podríamos trackear scopes
        return null;
    }
}
