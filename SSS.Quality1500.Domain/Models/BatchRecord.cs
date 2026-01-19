using SSS.Quality1500.Domain.Enums;

namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Modelo de dominio para registros de batch del archivo VK (DBF)
/// Implementa Value Object pattern para datos inmutables
/// </summary>
public class BatchRecord : BaseEntity
{
    #region Batch Information

    /// <summary>
    /// Número de batch de origen
    /// </summary>
    public string BatchFrom { get; set; } = string.Empty;

    /// <summary>
    /// Número de batch de destino
    /// </summary>
    public string BatchTo { get; set; } = string.Empty;

    /// <summary>
    /// Fecha del batch en formato MMDDYYYY
    /// </summary>
    public string BatchDate { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el batch tiene documentos asociados
    /// </summary>
    public bool HasDocuments { get; set; }

    #endregion

    #region Patient Information

    /// <summary>
    /// Nombre del paciente
    /// </summary>
    public string PatientName { get; set; } = string.Empty;

    /// <summary>
    /// Número de identificación del paciente
    /// </summary>
    public string? PatientId { get; set; }

    /// <summary>
    /// Fecha de nacimiento del paciente
    /// </summary>
    public DateTime? PatientDateOfBirth { get; set; }

    /// <summary>
    /// Género del paciente
    /// </summary>
    public string? PatientGender { get; set; }

    #endregion

    #region Service Information

    /// <summary>
    /// Fecha del servicio
    /// </summary>
    public DateTime ServiceDate { get; set; }

    /// <summary>
    /// Código del proveedor
    /// </summary>
    public string? ProviderCode { get; set; }

    /// <summary>
    /// Código del procedimiento
    /// </summary>
    public string? ProcedureCode { get; set; }

    /// <summary>
    /// Descripción del procedimiento
    /// </summary>
    public string? ProcedureDescription { get; set; }

    #endregion

    #region Financial Information

    /// <summary>
    /// Monto del claim
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Moneda del monto
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Número de autorización
    /// </summary>
    public string? AuthorizationNumber { get; set; }

    #endregion

    #region Processing Status

    /// <summary>
    /// Estado del procesamiento del batch
    /// </summary>
    public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;

    /// <summary>
    /// Fecha de procesamiento
    /// </summary>
    public DateTime? ProcessedDate { get; set; }

    /// <summary>
    /// Mensajes de error o información del procesamiento
    /// </summary>
    public string? ProcessingNotes { get; set; }

    /// <summary>
    /// Número de intentos de procesamiento
    /// </summary>
    public int ProcessingAttempts { get; set; } = 0;

    #endregion

    #region Domain Methods

    /// <summary>
    /// Valida que el registro tiene la información mínima requerida
    /// Principio Single Responsibility: Centraliza validación de negocio
    /// </summary>
    /// <returns>True si el registro es válido</returns>
    public bool IsValidRecord()
    {
        return !string.IsNullOrWhiteSpace(BatchFrom) &&
               !string.IsNullOrWhiteSpace(BatchTo) &&
               !string.IsNullOrWhiteSpace(BatchDate) &&
               !string.IsNullOrWhiteSpace(PatientName) &&
               ServiceDate != DateTime.MinValue &&
               Amount >= 0;
    }

    /// <summary>
    /// Marca el registro como procesado exitosamente
    /// Principio Command pattern para operaciones de estado
    /// </summary>
    /// <param name="processedBy">Usuario que procesó el registro</param>
    /// <param name="notes">Notas del procesamiento</param>
    public void MarkAsProcessed(string processedBy, string? notes = null)
    {
        Status = ProcessingStatus.Completed;
        ProcessedDate = DateTime.UtcNow;
        ProcessingNotes = notes;
        ProcessingAttempts++;
        MarkAsUpdated(processedBy);
    }

    /// <summary>
    /// Marca el registro como fallido en el procesamiento
    /// Principio Command pattern para operaciones de estado
    /// </summary>
    /// <param name="processedBy">Usuario que intentó procesar</param>
    /// <param name="errorMessage">Mensaje de error</param>
    public void MarkAsFailed(string processedBy, string errorMessage)
    {
        Status = ProcessingStatus.Failed;
        ProcessingNotes = errorMessage;
        ProcessingAttempts++;
        MarkAsUpdated(processedBy);
    }

    /// <summary>
    /// Reinicia el estado del registro para reprocesamiento
    /// Principio Command pattern para operaciones de estado
    /// </summary>
    /// <param name="resetBy">Usuario que reinicia el procesamiento</param>
    public void ResetForReprocessing(string resetBy)
    {
        Status = ProcessingStatus.Pending;
        ProcessedDate = null;
        ProcessingNotes = "Reset for reprocessing";
        MarkAsUpdated(resetBy);
    }

    /// <summary>
    /// Calcula la edad del paciente en la fecha del servicio
    /// Principio Single Responsibility: Lógica de cálculo encapsulada
    /// </summary>
    /// <returns>Edad del paciente o null si no hay fecha de nacimiento</returns>
    public int? CalculatePatientAge()
    {
        if (PatientDateOfBirth == null) return null;

        var age = ServiceDate.Year - PatientDateOfBirth.Value.Year;
        if (ServiceDate.Date < PatientDateOfBirth.Value.Date.AddYears(age))
            age--;

        return age;
    }

    /// <summary>
    /// Obtiene un resumen del registro para display
    /// Principio Single Responsibility: Formateo centralizado
    /// </summary>
    /// <returns>Resumen del registro</returns>
    public string GetDisplaySummary()
    {
        return $"Batch: {BatchFrom} | Patient: {PatientName} | Date: {ServiceDate:MM/dd/yyyy} | Amount: ${Amount:F2} | Status: {Status}";
    }

    #endregion
}

