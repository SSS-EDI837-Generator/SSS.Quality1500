using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Business.Events;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Presentation.ViewModels;

namespace SSS.Quality1500.Presentation.EventHandlers;

/// <summary>
/// Manejador de eventos para métricas de rendimiento y estadísticas
/// Principio SRP: Solo se encarga de calcular y actualizar métricas de rendimiento
/// Principio OCP: Extensible para nuevas métricas sin modificar código existente
/// </summary>
public class PerformanceMetricsEventHandler : IEventListener<ProgressEvent>
{
    private readonly ControlMainViewModel _viewModel;
    private readonly ILogger<PerformanceMetricsEventHandler> _logger;
    private readonly Dispatcher _dispatcher;
    private readonly Dictionary<string, ProcessMetrics> _processMetrics;
    private readonly object _metricsLock = new();

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="viewModel">ViewModel para actualizar</param>
    /// <param name="logger">Logger para diagnósticos</param>
    /// <param name="dispatcher">Dispatcher para thread-safe UI updates</param>
    public PerformanceMetricsEventHandler(
        ControlMainViewModel viewModel,
        ILogger<PerformanceMetricsEventHandler> logger,
        Dispatcher dispatcher)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _processMetrics = new Dictionary<string, ProcessMetrics>();
    }

    /// <summary>
    /// Maneja eventos de progreso calculando métricas de rendimiento
    /// </summary>
    /// <param name="progressEvent">Evento de progreso</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    public async Task Handle(ProgressEvent? progressEvent, CancellationToken cancellationToken = default)
    {
        try
        {

            if (progressEvent?.ProgressInfo == null)
            {
                return;
            }

            string processId = progressEvent.ProcessId;
            ProgressInfo progress = progressEvent.ProgressInfo;

            // Actualizar métricas de forma thread-safe
            ProcessMetrics metrics = UpdateProcessMetrics(processId, progress);

            // Actualizar UI con métricas calculadas (no bloqueante)
            await _dispatcher.BeginInvoke(() =>
            {
                try
                {
                    UpdateUiMetrics(metrics, progress);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error actualizando métricas UI para proceso {ProcessId}", processId);
                }
            }, DispatcherPriority.Background);

            // Completar inmediatamente para evitar bloqueos
            await Task.CompletedTask;

            _logger.LogTrace("Métricas actualizadas para proceso {ProcessId} - ETA: {ETA}, Velocidad: {Speed} rec/min",
                processId, metrics.EstimatedTimeToComplete, metrics.ProcessingSpeed);
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Actualización de métricas cancelada para proceso {ProcessId}", progressEvent?.ProcessId ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando métricas para proceso {ProcessId}", progressEvent?.ProcessId ?? "");
        }
    }

    /// <summary>
    /// Actualiza las métricas del proceso de forma thread-safe
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    /// <returns>Métricas actualizadas del proceso</returns>
    private ProcessMetrics UpdateProcessMetrics(string processId, ProgressInfo progress)
    {
        lock (_metricsLock)
        {
            if (!_processMetrics.TryGetValue(processId, out var metrics))
            {
                metrics = new ProcessMetrics
                {
                    ProcessId = processId,
                    StartTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    InitialRecordCount = progress.ProcessedRecords
                };
                _processMetrics[processId] = metrics;
            }

            // Actualizar métricas
            metrics.LastUpdateTime = DateTime.Now;
            metrics.CurrentRecordCount = progress.ProcessedRecords;
            metrics.TotalRecords = progress.TotalRecords;
            metrics.CurrentPercentage = progress.Percentage;
            metrics.CurrentPhase = progress.CurrentPhase;
            metrics.CurrentBatch = progress.CurrentBatch;

            // Calcular velocidad de procesamiento
            CalculateProcessingSpeed(metrics);

            // Calcular tiempo estimado de finalización
            CalculateEstimatedTimeToComplete(metrics);

            return metrics;
        }
    }

    /// <summary>
    /// Calcula la velocidad de procesamiento en registros por minuto
    /// </summary>
    /// <param name="metrics">Métricas del proceso</param>
    private void CalculateProcessingSpeed(ProcessMetrics metrics)
    {
        var elapsedTime = metrics.LastUpdateTime - metrics.StartTime;
        if (elapsedTime.TotalMinutes > 0)
        {
            var processedRecords = metrics.CurrentRecordCount - metrics.InitialRecordCount;
            metrics.ProcessingSpeed = processedRecords / elapsedTime.TotalMinutes;
        }
    }

    /// <summary>
    /// Calcula el tiempo estimado de finalización
    /// </summary>
    /// <param name="metrics">Métricas del proceso</param>
    private void CalculateEstimatedTimeToComplete(ProcessMetrics metrics)
    {
        if (metrics.ProcessingSpeed > 0 && metrics.TotalRecords > 0)
        {
            var remainingRecords = metrics.TotalRecords - metrics.CurrentRecordCount;
            var estimatedMinutes = remainingRecords / metrics.ProcessingSpeed;
            metrics.EstimatedTimeToComplete = DateTime.Now.AddMinutes(estimatedMinutes);
        }
    }

    /// <summary>
    /// Actualiza las métricas en la UI
    /// </summary>
    /// <param name="metrics">Métricas calculadas</param>
    /// <param name="progress">Información de progreso</param>
    private void UpdateUiMetrics(ProcessMetrics metrics, ProgressInfo progress)
    {
        // Actualizar tiempo de ejecución
        var executionTime = DateTime.Now - metrics.StartTime;
        _viewModel.ExecutionTime = $"{executionTime.TotalMilliseconds:F0} ms";

        // Actualizar ETA si está disponible
        if (metrics.EstimatedTimeToComplete.HasValue && progress.CurrentPhase != ProcessingPhase.Completed)
        {
            var eta = metrics.EstimatedTimeToComplete.Value;
            var remainingTime = eta - DateTime.Now;

            if (remainingTime.TotalSeconds > 0)
            {
                _viewModel.ProgressText = $"{progress.Message} - ETA: {eta:HH:mm:ss}";
            }
        }

        // Log métricas para diagnóstico
        _logger.LogDebug("Métricas de rendimiento - Velocidad: {Speed:F1} rec/min, Tiempo transcurrido: {Elapsed}, ETA: {ETA}",
            metrics.ProcessingSpeed, executionTime, metrics.EstimatedTimeToComplete);
    }
}

/// <summary>
/// Clase para almacenar métricas de un proceso
/// </summary>
internal class ProcessMetrics
{
    public string ProcessId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public int InitialRecordCount { get; set; }
    public int CurrentRecordCount { get; set; }
    public int TotalRecords { get; set; }
    public double CurrentPercentage { get; set; }
    public ProcessingPhase CurrentPhase { get; set; }
    public string? CurrentBatch { get; set; }
    public double ProcessingSpeed { get; set; } // registros por minuto
    public DateTime? EstimatedTimeToComplete { get; set; }
}
