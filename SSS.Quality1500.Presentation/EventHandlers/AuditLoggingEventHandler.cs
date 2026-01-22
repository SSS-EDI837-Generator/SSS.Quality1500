using Microsoft.Extensions.Logging;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Business.Events;
using SSS.Quality1500.Business.Models;

namespace SSS.Quality1500.Presentation.EventHandlers;

/// <summary>
/// Manejador de eventos para logging y auditoría de progreso
/// Principio SRP: Solo se encarga de registrar y auditar eventos de progreso
/// Principio OCP: Extensible para nuevos tipos de auditoría sin modificar código existente
/// </summary>
public class AuditLoggingEventHandler : IEventListener<ProgressEvent>
{
    private readonly ILogger<AuditLoggingEventHandler> _logger;
    private readonly Dictionary<string, ProcessAuditInfo> _processAudits;
    private readonly object _auditLock = new();

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="logger">Logger para diagnósticos y auditoría</param>
    public AuditLoggingEventHandler(ILogger<AuditLoggingEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _processAudits = new Dictionary<string, ProcessAuditInfo>();
    }

    /// <summary>
    /// Maneja eventos de progreso registrando información de auditoría
    /// </summary>
    /// <param name="progressEvent">Evento de progreso</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Task que representa la operación asíncrona</returns>
    public async Task Handle(ProgressEvent progressEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            if (progressEvent?.ProgressInfo == null)
            {
                return;
            }

            var processId = progressEvent.ProcessId;
            var progress = progressEvent.ProgressInfo;

            // Registrar auditoría del proceso
            UpdateProcessAudit(processId, progress, progressEvent.OperationType);

            // Logging específico basado en la fase
            await LogProgressByPhase(processId, progress, progressEvent.OperationType);

            // Logging estructurado para métricas y análisis
            LogStructuredProgress(processId, progress, progressEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en auditoría de progreso para proceso {ProcessId}", progressEvent.ProcessId);
        }
    }

    /// <summary>
    /// Actualiza la información de auditoría del proceso
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    /// <param name="operationType">Tipo de operación</param>
    private void UpdateProcessAudit(string processId, ProgressInfo progress, string operationType)
    {
        lock (_auditLock)
        {
            if (!_processAudits.TryGetValue(processId, out var auditInfo))
            {
                auditInfo = new ProcessAuditInfo
                {
                    ProcessId = processId,
                    OperationType = operationType,
                    StartTime = DateTime.UtcNow,
                    UserName = Environment.UserName,
                    MachineName = Environment.MachineName,
                    InitialRecordCount = progress.ProcessedRecords,
                    TotalRecords = progress.TotalRecords
                };
                _processAudits[processId] = auditInfo;

                // Log inicio del proceso
                _logger.LogInformation("Proceso iniciado - ID: {ProcessId}, Tipo: {OperationType}, Usuario: {UserName}, Máquina: {MachineName}, Total registros: {TotalRecords}",
                    processId, operationType, auditInfo.UserName, auditInfo.MachineName, progress.TotalRecords);
            }

            // Actualizar información
            auditInfo.LastUpdateTime = DateTime.UtcNow;
            auditInfo.CurrentPercentage = progress.Percentage;
            auditInfo.CurrentPhase = progress.CurrentPhase;
            auditInfo.CurrentBatch = progress.CurrentBatch;
            auditInfo.ProcessedRecords = progress.ProcessedRecords;
            auditInfo.LastMessage = progress.Message;

            // Detectar cambios de fase para logging especial
            if (auditInfo.PreviousPhase != progress.CurrentPhase)
            {
                LogPhaseTransition(processId, auditInfo.PreviousPhase, progress.CurrentPhase);
                auditInfo.PreviousPhase = progress.CurrentPhase;
            }
        }
    }

    /// <summary>
    /// Registra progreso específico basado en la fase actual
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    /// <param name="operationType">Tipo de operación</param>
    private async Task LogProgressByPhase(string processId, ProgressInfo progress, string operationType)
    {
        switch (progress.CurrentPhase)
        {
            case ProcessingPhase.Initializing:
                await LogInitializingPhase(processId, progress, operationType);
                break;
            case ProcessingPhase.ValidatingParameters:
                _logger.LogDebug("Validando parámetros - Proceso: {ProcessId}, Mensaje: {Message}", processId, progress.Message);
                break;
            case ProcessingPhase.ExtractingBatches:
                _logger.LogDebug("Extrayendo batches - Proceso: {ProcessId}, Progreso: {Progress}%", processId, progress.Percentage);
                break;
            case ProcessingPhase.TransformingClaims:
                await LogTransformingClaims(processId, progress);
                break;
            case ProcessingPhase.GeneratingFiles:
                _logger.LogDebug("Generando archivos - Proceso: {ProcessId}, Progreso: {Progress}%", processId, progress.Percentage);
                break;
            case ProcessingPhase.Completed:
                await LogCompletedPhase(processId, progress);
                break;
        }
    }

    /// <summary>
    /// Registra la fase de inicialización, incluyendo errores y cancelaciones
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    /// <param name="operationType">Tipo de operación</param>
    private async Task LogInitializingPhase(string processId, ProgressInfo progress, string operationType)
    {
        if (progress.Message.Contains("Error"))
        {
            _logger.LogError("Error en proceso {ProcessId} ({OperationType}): {Message}", processId, operationType, progress.Message);
        }
        else if (progress.Message.Contains("cancelado"))
        {
            _logger.LogWarning("Proceso cancelado {ProcessId} ({OperationType}): {Message}", processId, operationType, progress.Message);
        }
        else
        {
            _logger.LogInformation("Inicializando proceso {ProcessId} ({OperationType}): {Message}", processId, operationType, progress.Message);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Registra la fase de transformación de claims
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    private async Task LogTransformingClaims(string processId, ProgressInfo progress)
    {
        if (!string.IsNullOrEmpty(progress.CurrentBatch))
        {
            _logger.LogDebug("Transformando claims - Proceso: {ProcessId}, Batch: {BatchNumber}, Progreso: {Progress}%, Registros: {ProcessedRecords}/{TotalRecords}",
                processId, progress.CurrentBatch, progress.Percentage, progress.ProcessedRecords, progress.TotalRecords);
        }
        else
        {
            _logger.LogDebug("Transformando claims - Proceso: {ProcessId}, Progreso: {Progress}%, Registros: {ProcessedRecords}/{TotalRecords}",
                processId, progress.Percentage, progress.ProcessedRecords, progress.TotalRecords);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Registra la fase de completado con estadísticas finales
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    private async Task LogCompletedPhase(string processId, ProgressInfo progress)
    {
        lock (_auditLock)
        {
            if (_processAudits.TryGetValue(processId, out var auditInfo))
            {
                auditInfo.EndTime = DateTime.UtcNow;
                var totalTime = auditInfo.EndTime.Value - auditInfo.StartTime;
                var processedRecords = progress.ProcessedRecords - auditInfo.InitialRecordCount;

                _logger.LogInformation("Proceso completado - ID: {ProcessId}, Tiempo total: {TotalTime}, Registros procesados: {ProcessedRecords}, Velocidad: {Speed} rec/min",
                    processId, totalTime, processedRecords, processedRecords / totalTime.TotalMinutes);

                // Limpiar auditoría después de completar
                _processAudits.Remove(processId);
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Registra transiciones entre fases
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="fromPhase">Fase anterior</param>
    /// <param name="toPhase">Fase actual</param>
    private void LogPhaseTransition(string processId, ProcessingPhase fromPhase, ProcessingPhase toPhase)
    {
        _logger.LogInformation("Transición de fase - Proceso: {ProcessId}, De: {FromPhase} → A: {ToPhase}",
            processId, fromPhase, toPhase);
    }

    /// <summary>
    /// Registra progreso estructurado para análisis y métricas
    /// </summary>
    /// <param name="processId">ID del proceso</param>
    /// <param name="progress">Información de progreso</param>
    /// <param name="progressEvent">Evento completo</param>
    private void LogStructuredProgress(string processId, ProgressInfo progress, ProgressEvent progressEvent)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["ProcessId"] = processId,
            ["OperationType"] = progressEvent.OperationType,
            ["EventTimestamp"] = progressEvent.Timestamp,
            ["Percentage"] = progress.Percentage,
            ["Phase"] = progress.CurrentPhase,
            ["ProcessedRecords"] = progress.ProcessedRecords,
            ["TotalRecords"] = progress.TotalRecords,
            ["CurrentBatch"] = progress.CurrentBatch ?? "N/A"
        });

        _logger.LogTrace("Progreso estructurado registrado para análisis");
    }
}

/// <summary>
/// Información de auditoría de un proceso
/// </summary>
internal class ProcessAuditInfo
{
    public string ProcessId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public int InitialRecordCount { get; set; }
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public double CurrentPercentage { get; set; }
    public ProcessingPhase CurrentPhase { get; set; }
    public ProcessingPhase PreviousPhase { get; set; }
    public string? CurrentBatch { get; set; }
    public string LastMessage { get; set; } = string.Empty;
}
