namespace SSS.Quality1500.Business.Models;
/// <summary>
/// Builder para crear instancias de ProgressInfo de forma fluida
/// Implementa el patrón Builder para reducir la cantidad de parámetros
/// Principio SRP: Se encarga únicamente de construir objetos ProgressInfo
/// Mantiene la inmutabilidad del objeto ProgressInfo
/// </summary>
public class ProgressInfoBuilder
{
    private double _percentage;
    private string _message = string.Empty;
    private int _processedRecords = 0;
    private int _totalRecords = 0;
    private ProcessingPhase _currentPhase = ProcessingPhase.Initializing;
    private string? _currentBatch = null;
    
    /// <summary>
    /// Establece el porcentaje de progreso
    /// </summary>
    /// <param name="percentage">Porcentaje de progreso (0-100)</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithPercentage(double percentage)
    {
        _percentage = percentage;
        return this;
    }
    
    /// <summary>
    /// Establece el mensaje descriptivo del estado actual
    /// </summary>
    /// <param name="message">Mensaje descriptivo</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithMessage(string message)
    {
        _message = message ?? string.Empty;
        return this;
    }
    
    /// <summary>
    /// Establece el número de registros procesados
    /// </summary>
    /// <param name="processedRecords">Número de registros procesados</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithProcessedRecords(int processedRecords)
    {
        _processedRecords = processedRecords;
        return this;
    }
    
    /// <summary>
    /// Establece el número total de registros
    /// </summary>
    /// <param name="totalRecords">Número total de registros</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithTotalRecords(int totalRecords)
    {
        _totalRecords = totalRecords;
        return this;
    }
    
    /// <summary>
    /// Establece la fase actual del procesamiento
    /// </summary>
    /// <param name="currentPhase">Fase actual del procesamiento</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithCurrentPhase(ProcessingPhase currentPhase)
    {
        _currentPhase = currentPhase;
        return this;
    }
    
    /// <summary>
    /// Establece el batch actual siendo procesado
    /// </summary>
    /// <param name="currentBatch">Batch actual siendo procesado</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithCurrentBatch(string? currentBatch)
    {
        _currentBatch = currentBatch;
        return this;
    }
    
    /// <summary>
    /// Establece el progreso basado en los registros procesados
    /// </summary>
    /// <param name="processedRecords">Registros procesados</param>
    /// <param name="totalRecords">Total de registros</param>
    /// <returns>Builder para encadenamiento fluido</returns>
    public ProgressInfoBuilder WithRecordsProgress(int processedRecords, int totalRecords)
    {
        _processedRecords = processedRecords;
        _totalRecords = totalRecords;
        if (totalRecords > 0)
        {
            _percentage = (double)processedRecords / totalRecords * 100;
        }
        return this;
    }
    
    /// <summary>
    /// Construye la instancia final de ProgressInfo
    /// </summary>
    /// <returns>Instancia inmutable de ProgressInfo</returns>
    public ProgressInfo Build()
    {
        return new ProgressInfo(
            _percentage,
            _message,
            _processedRecords,
            _totalRecords,
            _currentPhase,
            _currentBatch
        );
    }
    
    /// <summary>
    /// Crea un nuevo builder
    /// </summary>
    /// <returns>Nueva instancia del builder</returns>
    public static ProgressInfoBuilder Create() => new();
    
    /// <summary>
    /// Crea un builder con valores básicos pre-configurados
    /// </summary>
    /// <param name="percentage">Porcentaje de progreso</param>
    /// <param name="message">Mensaje descriptivo</param>
    /// <returns>Builder con valores básicos configurados</returns>
    public static ProgressInfoBuilder Create(double percentage, string message) =>
        new ProgressInfoBuilder()
            .WithPercentage(percentage)
            .WithMessage(message);
    
    /// <summary>
    /// Crea un builder para progreso de inicialización
    /// </summary>
    /// <param name="message">Mensaje de inicialización</param>
    /// <returns>Builder configurado para inicialización</returns>
    public static ProgressInfoBuilder Initializing(string message) =>
        new ProgressInfoBuilder()
            .WithPercentage(0)
            .WithMessage(message)
            .WithCurrentPhase(ProcessingPhase.Initializing);
            
    /// <summary>
    /// Crea un builder para progreso completado
    /// </summary>
    /// <param name="message">Mensaje de completado</param>
    /// <returns>Builder configurado para completado</returns>
    public static ProgressInfoBuilder Completed(string message) =>
        new ProgressInfoBuilder()
            .WithPercentage(100)
            .WithMessage(message)
            .WithCurrentPhase(ProcessingPhase.Completed);
}
