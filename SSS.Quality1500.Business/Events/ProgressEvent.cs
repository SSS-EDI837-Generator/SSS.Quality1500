namespace SSS.Quality1500.Business.Events;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Business.Models;



/// <summary>
/// Evento que se publica cuando hay actualizaciones de progreso en el procesamiento
/// Principio SRP: Solo contiene información del progreso
/// Desacopla el core de negocio de la UI
/// </summary>
public class ProgressEvent : IEvent
{
    /// <summary>
    /// Timestamp cuando el evento fue creado
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Información del progreso
    /// </summary>
    public ProgressInfo ProgressInfo { get; }

    /// <summary>
    /// Identificador único del proceso que reporta el progreso
    /// Permite distinguir entre múltiples procesos concurrentes
    /// </summary>
    public string ProcessId { get; }

    /// <summary>
    /// Tipo de operación que está reportando el progreso
    /// </summary>
    public string OperationType { get; }

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="progressInfo">Información del progreso</param>
    /// <param name="processId">Identificador del proceso</param>
    /// <param name="operationType">Tipo de operación</param>
    public ProgressEvent(ProgressInfo progressInfo, string processId, string operationType)
    {
        Timestamp = DateTime.UtcNow;
        ProgressInfo = progressInfo ?? throw new ArgumentNullException(nameof(progressInfo));
        ProcessId = processId ?? throw new ArgumentNullException(nameof(processId));
        OperationType = operationType ?? throw new ArgumentNullException(nameof(operationType));
    }

    /// <summary>
    /// Método de conveniencia para crear un evento de progreso de procesamiento 837P
    /// </summary>
    /// <param name="progressInfo">Información del progreso</param>
    /// <param name="processId">Identificador del proceso</param>
    /// <returns>Evento de progreso configurado</returns>
    public static ProgressEvent For837Processing(ProgressInfo progressInfo, string processId)
    {
        return new ProgressEvent(progressInfo, processId, "837P_Processing");
    }

    /// <summary>
    /// Método de conveniencia para crear un evento de progreso de transformación de claims
    /// </summary>
    /// <param name="progressInfo">Información del progreso</param>
    /// <param name="processId">Identificador del proceso</param>
    /// <returns>Evento de progreso configurado</returns>
    public static ProgressEvent ForClaimTransformation(ProgressInfo progressInfo, string processId)
    {
        return new ProgressEvent(progressInfo, processId, "Claim_Transformation");
    }

    /// <summary>
    /// Método de conveniencia para crear un evento de progreso de generación de archivos
    /// </summary>
    /// <param name="progressInfo">Información del progreso</param>
    /// <param name="processId">Identificador del proceso</param>
    /// <returns>Evento de progreso configurado</returns>
    public static ProgressEvent ForFileGeneration(ProgressInfo progressInfo, string processId)
    {
        return new ProgressEvent(progressInfo, processId, "File_Generation");
    }

    /// <summary>
    /// Representación en cadena del evento
    /// </summary>
    /// <returns>Descripción del evento</returns>
    public override string ToString()
    {
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {OperationType} ({ProcessId}): {ProgressInfo.Percentage:F1}% - {ProgressInfo.Message}";
    }
}
