namespace SSS.Quality1500.Business.Models;

using System.Data;


/// <summary>
/// Objeto de configuración para el proceso Start837Process
/// Implementa el patrón Parameter Object para reducir parámetros del método
/// Principio SRP: Encapsula toda la configuración necesaria para el proceso
/// </summary>
public class StartProcessRequest
{
    /// <summary>
    /// Tabla VDE con los datos a procesar
    /// </summary>
    public DataTable VdeTable { get; set; }

    /// <summary>
    /// Ruta de salida para los archivos generados
    /// </summary>
    public string PathDat { get; set; }

    /// <summary>
    /// Nombre del archivo de salida 837P
    /// </summary>
    public string FileNameOutPut837 { get; set; }

    /// <summary>
    /// Año de procesamiento (4 dígitos)
    /// </summary>
    public string Year { get; set; }

    /// <summary>
    /// Lista de batches seleccionados para procesamiento
    /// </summary>
    public List<string> SelectedBatches { get; set; }

    /// <summary>
    /// Token de cancelación para el proceso
    /// </summary>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// Constructor primario
    /// </summary>
    /// <param name="vdeTable">Tabla VDE con datos</param>
    /// <param name="pathDat">Ruta de salida</param>
    /// <param name="fileNameOutPut837">Nombre del archivo de salida</param>
    public StartProcessRequest(DataTable vdeTable, string pathDat, string fileNameOutPut837)
    {
        VdeTable = vdeTable;
        PathDat = pathDat;
        FileNameOutPut837 = fileNameOutPut837;
        Year = string.Empty;
        SelectedBatches = new List<string>();
    }

    /// <summary>
    /// Valida si el request tiene la configuración mínima requerida
    /// </summary>
    /// <returns>True si la configuración es válida</returns>
    public bool IsValid()
    {
        return VdeTable != null && 
               VdeTable.Rows.Count > 0 &&
               !string.IsNullOrWhiteSpace(PathDat) &&
               !string.IsNullOrWhiteSpace(FileNameOutPut837) &&
               !string.IsNullOrWhiteSpace(Year) &&
               Year.Length == 4 &&
               SelectedBatches != null &&
               SelectedBatches.Count > 0;
    }

    /// <summary>
    /// Obtiene los errores de validación específicos
    /// </summary>
    /// <returns>Lista de errores de validación</returns>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (VdeTable == null || VdeTable.Rows.Count == 0)
            errors.Add("La tabla VDE está vacía o es nula");

        if (string.IsNullOrWhiteSpace(PathDat))
            errors.Add("La ruta de salida es requerida");

        if (string.IsNullOrWhiteSpace(FileNameOutPut837))
            errors.Add("El nombre del archivo de salida es requerido");

        if (string.IsNullOrWhiteSpace(Year))
            errors.Add("El año es requerido");
        else if (Year.Length != 4)
            errors.Add("El año debe tener 4 dígitos");

        if (SelectedBatches == null || SelectedBatches.Count == 0)
            errors.Add("La lista de batches seleccionados no puede estar vacía");

        return errors;
    }

    /// <summary>
    /// Método fluent para configurar el año
    /// </summary>
    /// <param name="year">Año de procesamiento</param>
    /// <returns>La instancia actual para encadenamiento</returns>
    public StartProcessRequest WithYear(string year)
    {
        Year = year;
        return this;
    }

    /// <summary>
    /// Método fluent para configurar batches seleccionados
    /// </summary>
    /// <param name="selectedBatches">Lista de batches</param>
    /// <returns>La instancia actual para encadenamiento</returns>
    public StartProcessRequest WithSelectedBatches(IEnumerable<string> selectedBatches)
    {
        SelectedBatches = selectedBatches?.ToList() ?? new List<string>();
        return this;
    }

    /// <summary>
    /// Método fluent para configurar el token de cancelación
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>La instancia actual para encadenamiento</returns>
    public StartProcessRequest WithCancellationToken(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
        return this;
    }
}
