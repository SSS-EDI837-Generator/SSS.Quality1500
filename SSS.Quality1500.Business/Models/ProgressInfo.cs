namespace SSS.Quality1500.Business.Models;

/// <summary>
/// Información de progreso para el procesamiento de archivos 837P
/// </summary>
public class ProgressInfo
{
    /// <summary>
    /// Porcentaje de progreso (0-100)
    /// </summary>
    public double Percentage { get; set; }
    
    /// <summary>
    /// Mensaje descriptivo del estado actual
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Número de registros procesados hasta el momento
    /// </summary>
    public int ProcessedRecords { get; set; }
    
    /// <summary>
    /// Número total de registros a procesar
    /// </summary>
    public int TotalRecords { get; set; }
    
    /// <summary>
    /// Fase actual del procesamiento
    /// </summary>
    public ProcessingPhase CurrentPhase { get; set; }
    
    /// <summary>
    /// Batch actual siendo procesado
    /// </summary>
    public string? CurrentBatch { get; set; }

    /// <summary>
    /// Constructor interno para uso exclusivo del ProgressInfoBuilder
    /// Se hizo interno para forzar el uso del patrón Builder y cumplir con la regla de máximo 3 parámetros
    /// </summary>
    internal ProgressInfo(double percentage, string message, int processedRecords, int totalRecords, 
                         ProcessingPhase currentPhase, string? currentBatch)
    {
        Percentage = Math.Max(0, Math.Min(100, percentage));
        Message = message;
        ProcessedRecords = processedRecords;
        TotalRecords = totalRecords;
        CurrentPhase = currentPhase;
        CurrentBatch = currentBatch;
    }
    
    /// <summary>
    /// Crea un nuevo builder para ProgressInfo
    /// Método de conveniencia para usar el patrón Builder
    /// </summary>
    /// <returns>Nueva instancia del builder</returns>
    public static ProgressInfoBuilder Builder() => ProgressInfoBuilder.Create();
    
    /// <summary>
    /// Crea un nuevo builder con valores básicos pre-configurados
    /// Método de conveniencia para usar el patrón Builder
    /// </summary>
    /// <param name="percentage">Porcentaje de progreso</param>
    /// <param name="message">Mensaje descriptivo</param>
    /// <returns>Builder con valores básicos configurados</returns>
    public static ProgressInfoBuilder Builder(double percentage, string message) => 
        ProgressInfoBuilder.Create(percentage, message);
    
    /// <summary>
    /// Crea un progreso de inicialización usando el builder
    /// </summary>
    /// <param name="message">Mensaje de inicialización</param>
    /// <returns>Builder configurado para inicialización</returns>
    public static ProgressInfoBuilder Initializing(string message) => 
        ProgressInfoBuilder.Initializing(message);
        
    /// <summary>
    /// Crea un progreso completado usando el builder
    /// </summary>
    /// <param name="message">Mensaje de completado</param>
    /// <returns>Builder configurado para completado</returns>
    public static ProgressInfoBuilder Completed(string message) => 
        ProgressInfoBuilder.Completed(message);
}

/// <summary>
/// Fases del procesamiento 837P
/// </summary>
public enum ProcessingPhase
{
    Initializing,
    ValidatingParameters,
    ExtractingBatches,
    TransformingClaims,
    GeneratingFiles,
    Completed
}
