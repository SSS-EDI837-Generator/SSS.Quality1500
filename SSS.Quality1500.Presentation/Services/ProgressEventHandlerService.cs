namespace SSS.Quality1500.Presentation.Services;

using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Business.Events;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Presentation.EventHandlers;
using SSS.Quality1500.Presentation.ViewModels;


/// <summary>
/// Servicio para registrar y gestionar manejadores de eventos de progreso
/// Principio SRP: Solo se encarga de la gestión de suscripciones a eventos
/// Principio Facade: Proporciona una interfaz simplificada para el manejo de eventos
/// </summary>
public class ProgressEventHandlerService : IDisposable
{
    private readonly IEventAggregator _eventAggregator;
    private readonly ILogger<ProgressEventHandlerService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly List<IEventHandler<ProgressEvent>> _handlers;
    private bool _disposed = false;

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="eventAggregator">Event Aggregator para suscripciones</param>
    /// <param name="logger">Logger para diagnósticos</param>
    /// <param name="loggerFactory">Factory para crear loggers</param>
    public ProgressEventHandlerService(
        IEventAggregator eventAggregator,
        ILogger<ProgressEventHandlerService> logger,
        ILoggerFactory loggerFactory)
    {
        _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _handlers = new List<IEventHandler<ProgressEvent>>();
    }

    /// <summary>
    /// Registra todos los manejadores de eventos para un ViewModel específico
    /// </summary>
    /// <param name="viewModel">ViewModel que recibirá actualizaciones</param>
    /// <param name="dispatcher">Dispatcher para actualizaciones thread-safe</param>
    public void RegisterHandlers(ControlMainViewModel viewModel, Dispatcher dispatcher)
    {
        try
        {
            _logger.LogInformation("Registrando manejadores de eventos de progreso para {ViewModelType}",
                viewModel.GetType().Name);

            // Crear manejadores específicos
            var progressHandler = new ProgressEventHandler(viewModel,
                _loggerFactory.CreateLogger<ProgressEventHandler>(), dispatcher);

            var metricsHandler = new PerformanceMetricsEventHandler(viewModel,
                _loggerFactory.CreateLogger<PerformanceMetricsEventHandler>(), dispatcher);

            var auditHandler = new AuditLoggingEventHandler(
                _loggerFactory.CreateLogger<AuditLoggingEventHandler>());

            // Registrar manejadores en el Event Aggregator
            _eventAggregator.Subscribe<ProgressEvent>(progressHandler);
            _eventAggregator.Subscribe<ProgressEvent>(metricsHandler);
            _eventAggregator.Subscribe<ProgressEvent>(auditHandler);

            // Mantener referencias para poder desuscribir después
            _handlers.Add(progressHandler);
            _handlers.Add(metricsHandler);
            _handlers.Add(auditHandler);

            _logger.LogInformation("Registrados {HandlerCount} manejadores de eventos de progreso",
                _handlers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registrando manejadores de eventos de progreso");
            throw;
        }
    }

    /// <summary>
    /// Desregistra todos los manejadores de eventos
    /// </summary>
    public void UnregisterHandlers()
    {
        try
        {
            _logger.LogInformation("Desregistrando {HandlerCount} manejadores de eventos", _handlers.Count);

            foreach (var handler in _handlers)
            {
                _eventAggregator.Unsubscribe<ProgressEvent>(handler);
            }

            _handlers.Clear();
            _logger.LogInformation("Manejadores de eventos desregistrados exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error desregistrando manejadores de eventos");
        }
    }

    /// <summary>
    /// Obtiene estadísticas de los manejadores registrados
    /// </summary>
    /// <returns>Información estadística</returns>
    public ProgressHandlerStatistics GetStatistics()
    {
        return new ProgressHandlerStatistics
        {
            RegisteredHandlers = _handlers.Count,
            EventAggregatorStats = new Dictionary<string, object> { ["HandlerCount"] = _handlers.Count },
            IsDisposed = _disposed
        };
    }

    /// <summary>
    /// Libera recursos utilizados por el servicio
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                UnregisterHandlers();
                _disposed = true;
                _logger.LogInformation("ProgressEventHandlerService disposed correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante dispose de ProgressEventHandlerService");
            }
        }
    }
}

/// <summary>
/// Estadísticas de los manejadores de eventos
/// </summary>
public class ProgressHandlerStatistics
{
    /// <summary>
    /// Número de manejadores registrados
    /// </summary>
    public int RegisteredHandlers { get; set; }

    /// <summary>
    /// Estadísticas del Event Aggregator
    /// </summary>
    public Dictionary<string, object> EventAggregatorStats { get; set; } = new();

    /// <summary>
    /// Indica si el servicio ha sido disposed
    /// </summary>
    public bool IsDisposed { get; set; }
}
