namespace SSS.Quality1500.Presentation.EventHandlers;

using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Business.Events;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Presentation.ViewModels;


/// <summary>
/// Manejador de eventos de progreso para actualizar la UI
/// Principio SRP: Solo se encarga de actualizar el progreso en el ViewModel
/// Principio DIP: Depende de abstracciones, no de implementaciones concretas
/// Thread-safe para actualizaciones de UI desde background threads
/// </summary>
public class ProgressEventHandler : IEventListener<ProgressEvent>
{
    private readonly ControlMainViewModel _viewModel;
    private readonly ILogger<ProgressEventHandler> _logger;
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="viewModel">ViewModel para actualizar</param>
    /// <param name="logger">Logger para diagnósticos</param>
    /// <param name="dispatcher">Dispatcher para thread-safe UI updates</param>
    public ProgressEventHandler(
        ControlMainViewModel viewModel,
        ILogger<ProgressEventHandler> logger,
        Dispatcher dispatcher)
    {
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
    }

    /// <summary>
    /// Maneja eventos de progreso actualizando la UI de forma thread-safe
    /// </summary>
    /// <param name="progressEvent">Evento de progreso</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    public async Task Handle(ProgressEvent progressEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar que el evento no sea null
            if (progressEvent?.ProgressInfo == null)
            {
                _logger.LogWarning("Evento de progreso null recibido");
                return;
            }

            ProgressInfo progress = progressEvent.ProgressInfo;

            _logger.LogTrace("Procesando evento de progreso: {ProcessId} - {Percentage}% - {Message}",
                progressEvent.ProcessId, progress.Percentage, progress.Message);

            // Actualizar UI en el thread principal de forma thread-safe (no bloqueante)
            await _dispatcher.BeginInvoke(() =>
             {
                 try
                 {
                     // Actualizar progreso principal
                     _viewModel.Progress = progress.Percentage;
                     _viewModel.ProgressText = progress.Message;
                     _viewModel.ProcessedRecords = progress.ProcessedRecords.ToString();

                     // Actualizar estado basado en la fase actual
                     UpdateStatusBasedOnPhase(progress);

                     // Log para diagnóstico
                     _logger.LogDebug("UI actualizada - Progreso: {Progress}% - Registros: {ProcessedRecords}/{TotalRecords}",
                         progress.Percentage, progress.ProcessedRecords, progress.TotalRecords);
                 }
                 catch (Exception ex)
                 {
                     _logger.LogError(ex, "Error actualizando UI para proceso {ProcessId}", progressEvent.ProcessId);
                 }
             }, DispatcherPriority.Render);

            // Completar inmediatamente para evitar bloqueos
            await Task.CompletedTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Actualización de progreso cancelada para proceso {ProcessId}", progressEvent.ProcessId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando evento de progreso para proceso {ProcessId}", progressEvent.ProcessId);
        }
    }

    /// <summary>
    /// Actualiza el estado del ViewModel basado en la fase de procesamiento
    /// </summary>
    /// <param name="progress">Información de progreso</param>
    private void UpdateStatusBasedOnPhase(ProgressInfo progress)
    {
        switch (progress.CurrentPhase)
        {
            case ProcessingPhase.Initializing:
                HandleInitializingPhase(progress);
                break;
            case ProcessingPhase.ValidatingParameters:
                _viewModel.Status = "Validando parámetros";
                break;
            case ProcessingPhase.ExtractingBatches:
                _viewModel.Status = "Extrayendo batches";
                break;
            case ProcessingPhase.TransformingClaims:
                _viewModel.Status = "Transformando claims";
                break;
            case ProcessingPhase.GeneratingFiles:
                _viewModel.Status = "Generando archivos";
                break;
            case ProcessingPhase.Completed:
                HandleCompletedPhase(progress);
                break;
            default:
                _viewModel.Status = "Procesando";
                break;
        }
    }

    /// <summary>
    /// Maneja la fase de inicialización, incluyendo errores y cancelaciones
    /// </summary>
    /// <param name="progress">Información de progreso</param>
    private void HandleInitializingPhase(ProgressInfo progress)
    {
        if (progress.Message.Contains("Error"))
        {
            _viewModel.Status = "Error";
            _viewModel.StatusColor = System.Windows.Media.Brushes.Red;
            _logger.LogWarning("Error detectado en progreso: {Message}", progress.Message);
        }
        else if (progress.Message.Contains("cancelado"))
        {
            _viewModel.Status = "Cancelado";
            _viewModel.StatusColor = System.Windows.Media.Brushes.Orange;
            _logger.LogInformation("Proceso cancelado: {Message}", progress.Message);
        }
        else
        {
            _viewModel.Status = "Iniciando";
            _viewModel.StatusColor = System.Windows.Media.Brushes.Orange;
        }
    }

    /// <summary>
    /// Maneja la fase de completado
    /// </summary>
    /// <param name="progress">Información de progreso</param>
    private void HandleCompletedPhase(ProgressInfo progress)
    {
        _viewModel.Status = "Completado";
        _viewModel.StatusColor = System.Windows.Media.Brushes.Green;
        _viewModel.Progress = 100;
        _viewModel.LastProcessTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        _logger.LogInformation("Proceso completado exitosamente: {Message}", progress.Message);
    }
}
