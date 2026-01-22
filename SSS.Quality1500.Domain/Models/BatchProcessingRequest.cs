namespace SSS.Quality1500.Domain.Models;

using System.Data;


/// <summary>
/// Objeto Parameter Object para encapsular los parámetros del método MyListBatch
/// Implementa el patrón Parameter Object para reducir la cantidad de parámetros
/// Principio SRP: Se encarga únicamente de encapsular parámetros de procesamiento de batches
/// Cumple con la regla de máximo 3 parámetros en constructores y métodos
/// </summary>
public class BatchProcessingRequest
{
    /// <summary>
    /// Tabla de datos VDE a procesar
    /// </summary>
    public DataTable VdeTable { get; set; }

    /// <summary>
    /// Lista donde se almacenarán los batch items
    /// </summary>
    public List<string> BatchList { get; set; }

    /// <summary>
    /// Nombre del campo de página (generalmente "V1Page")
    /// </summary>
    public string PageFieldName { get; set; }

    /// <summary>
    /// Ruta y nombre del archivo VDE (sin extensión)
    /// </summary>
    public string VdeFilePath { get; set; }

    /// <summary>
    /// Fecha juliana para el procesamiento (formato JJJ)
    /// </summary>
    public string JulianDate { get; set; }

    /// <summary>
    /// Indica si se debe usar la fecha juliana en el número de batch
    /// </summary>
    public bool UseJulianDateInBatchNumber { get; set; }

    /// <summary>
    /// Constructor principal del BatchProcessingRequest
    /// </summary>
    /// <param name="vdeTable">Tabla de datos VDE</param>
    /// <param name="batchList">Lista de batches</param>
    /// <param name="pageFieldName">Nombre del campo de página</param>
    public BatchProcessingRequest(DataTable vdeTable, List<string> batchList, string pageFieldName)
    {
        VdeTable = vdeTable ?? throw new ArgumentNullException(nameof(vdeTable));
        BatchList = batchList ?? throw new ArgumentNullException(nameof(batchList));
        PageFieldName = pageFieldName ?? throw new ArgumentNullException(nameof(pageFieldName));
        VdeFilePath = string.Empty;
        JulianDate = string.Empty;
        UseJulianDateInBatchNumber = false;
    }

    /// <summary>
    /// Constructor con parámetros adicionales
    /// </summary>
    /// <param name="vdeTable">Tabla de datos VDE</param>
    /// <param name="batchList">Lista de batches</param>
    /// <param name="config">Configuración adicional del procesamiento</param>
    public BatchProcessingRequest(DataTable vdeTable, List<string> batchList, BatchProcessingConfig config)
    {
        VdeTable = vdeTable ?? throw new ArgumentNullException(nameof(vdeTable));
        BatchList = batchList ?? throw new ArgumentNullException(nameof(batchList));
        ArgumentNullException.ThrowIfNull(config);

        PageFieldName = config.PageFieldName;
        VdeFilePath = config.VdeFilePath;
        JulianDate = config.JulianDate;
        UseJulianDateInBatchNumber = config.UseJulianDateInBatchNumber;
    }

    /// <summary>
    /// Método para crear una instancia con configuración fluida
    /// </summary>
    /// <param name="vdeTable">Tabla de datos VDE</param>
    /// <param name="batchList">Lista de batches</param>
    /// <param name="pageFieldName">Nombre del campo de página</param>
    /// <returns>Nueva instancia de BatchProcessingRequest</returns>
    public static BatchProcessingRequest Create(DataTable vdeTable, List<string> batchList, string pageFieldName)
    {
        return new BatchProcessingRequest(vdeTable, batchList, pageFieldName);
    }

    /// <summary>
    /// Método para configurar la ruta del archivo VDE
    /// </summary>
    /// <param name="vdeFilePath">Ruta del archivo VDE</param>
    /// <returns>Nueva instancia con la configuración aplicada</returns>
    public BatchProcessingRequest WithVdeFilePath(string vdeFilePath)
    {
        return new BatchProcessingRequest(VdeTable, BatchList, PageFieldName)
        {
            VdeFilePath = vdeFilePath ?? string.Empty,
            JulianDate = JulianDate,
            UseJulianDateInBatchNumber = UseJulianDateInBatchNumber
        };
    }

    /// <summary>
    /// Método para configurar la fecha juliana
    /// </summary>
    /// <param name="julianDate">Fecha juliana</param>
    /// <returns>Nueva instancia con la configuración aplicada</returns>
    public BatchProcessingRequest WithJulianDate(string julianDate)
    {
        return new BatchProcessingRequest(VdeTable, BatchList, PageFieldName)
        {
            VdeFilePath = VdeFilePath,
            JulianDate = julianDate ?? string.Empty,
            UseJulianDateInBatchNumber = UseJulianDateInBatchNumber
        };
    }

    /// <summary>
    /// Método para configurar el uso de fecha juliana en batch number
    /// </summary>
    /// <param name="useJulianDate">Indica si usar fecha juliana</param>
    /// <returns>Nueva instancia con la configuración aplicada</returns>
    public BatchProcessingRequest WithJulianDateInBatchNumber(bool useJulianDate)
    {
        return new BatchProcessingRequest(VdeTable, BatchList, PageFieldName)
        {
            VdeFilePath = VdeFilePath,
            JulianDate = JulianDate,
            UseJulianDateInBatchNumber = useJulianDate
        };
    }

    /// <summary>
    /// Valida que la request tenga todos los datos necesarios
    /// </summary>
    /// <returns>Lista de errores de validación</returns>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (VdeTable == null)
            errors.Add("VdeTable no puede ser null");
        else if (VdeTable.Rows.Count == 0)
            errors.Add("VdeTable no puede estar vacía");

        if (BatchList == null)
            errors.Add("BatchList no puede ser null");

        if (string.IsNullOrWhiteSpace(PageFieldName))
            errors.Add("PageFieldName no puede estar vacío");

        return errors;
    }

    /// <summary>
    /// Indica si la request es válida
    /// </summary>
    /// <returns>True si es válida, false en caso contrario</returns>
    public bool IsValid()
    {
        return GetValidationErrors().Count == 0;
    }
}

/// <summary>
/// Configuración adicional para el procesamiento de batches
/// Ayuda a reducir parámetros en el constructor principal
/// </summary>
public record BatchProcessingConfig(
    string PageFieldName,
    string VdeFilePath = "",
    string JulianDate = "",
    bool UseJulianDateInBatchNumber = false
);
