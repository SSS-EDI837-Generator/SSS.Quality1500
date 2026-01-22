namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Modelo de dominio para la configuración del sistema 
/// Implementa principios de Domain-Driven Design y Clean Architecture
/// </summary>
public class ConfigurationSystem : BaseEntity
{
    #region File Paths (Value Objects pattern)

    /// <summary>
    /// Ruta del archivo VK (DBF)
    /// </summary>
    public string VkFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del archivo DAT
    /// </summary>
    public string DatFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del archivo IDX
    /// </summary>
    public string IdxFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Ruta del archivo IMG de entrada
    /// </summary>
    public string? ImgFileInPath { get; set; }

    /// <summary>
    /// Ruta del archivo IMG de salida
    /// </summary>
    public string? ImgFileOutPath { get; set; }

    #endregion

    #region Processing Options (Strategy pattern)

    /// <summary>
    /// Opción de procesamiento:  Stamping
    /// </summary>
    public bool Enabletamping { get; set; }

    /// <summary>
    /// Opción de procesamiento: IDA
    /// </summary>
    public bool EnableIda { get; set; }

    /// <summary>
    /// Opción de procesamiento: Zipper
    /// </summary>
    public bool EnableZipper { get; set; }

    /// <summary>
    /// Opción de procesamiento: Conduce
    /// </summary>
    public bool EnableConduce { get; set; }

    /// <summary>
    /// Opción de procesamiento: Backup
    /// </summary>
    public bool EnableBackup { get; set; }

    /// <summary>
    /// Opción de procesamiento: Conduce Dummy
    /// </summary>
    public bool EnableConduceDummy { get; set; }

    /// <summary>
    /// Opción de procesamiento: VDVERSION
    /// </summary>
    public bool EnableVersion { get; set; }

    /// <summary>
    /// Opción de procesamiento: Remove Header
    /// </summary>
    public bool EnableRemoveHeader { get; set; }

    /// <summary>
    /// Opción de procesamiento: 837 to Xls
    /// </summary>
    public bool Enable837ToXls { get; set; }

    #endregion

    #region Metadata

    /// <summary>
    /// Fecha seleccionada para el procesamiento
    /// </summary>
    public DateTime ProcessingDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Total de imágenes configuradas
    /// </summary>
    public int TotalImages { get; set; }

    /// <summary>
    /// Total de documentos configurados
    /// </summary>
    public int TotalDocuments { get; set; }

    /// <summary>
    /// Nombre descriptivo de la configuración
    /// </summary>
    public string ConfigurationName { get; set; } = string.Empty;

    /// <summary>
    /// Descripción opcional de la configuración
    /// </summary>
    public string? Description { get; set; }

    #endregion

    #region Domain Methods (Business Logic)

    /// <summary>
    /// Valida que la configuración está completa
    /// Principio Single Responsibility: Centraliza validación de negocio
    /// </summary>
    /// <returns>True si la configuración es válida</returns>
    public bool IsConfigurationValid()
    {
        return !string.IsNullOrWhiteSpace(VkFilePath) &&
               !string.IsNullOrWhiteSpace(DatFilePath) &&
               !string.IsNullOrWhiteSpace(IdxFilePath) &&
               !string.IsNullOrWhiteSpace(ConfigurationName);
    }

    /// <summary>
    /// Obtiene las opciones de procesamiento habilitadas
    /// Principio Open/Closed: Extensible para nuevas opciones
    /// </summary>
    /// <returns>Lista de opciones habilitadas</returns>
    public List<string> GetEnabledProcessingOptions()
    {
        var options = new List<string>();

        if (Enabletamping) options.Add(" Stamping");
        if (EnableIda) options.Add("IDA");
        if (EnableZipper) options.Add("Zipper");
        if (EnableConduce) options.Add("Conduce");
        if (EnableBackup) options.Add("Backup");
        if (EnableConduceDummy) options.Add("Conduce Dummy");
        if (EnableVersion) options.Add("VDVERSION");
        if (EnableRemoveHeader) options.Add("Remove Header");
        if (Enable837ToXls) options.Add("data to Xls");

        return options;
    }

    /// <summary>
    /// Crea una copia de la configuración para backup
    /// Principio Prototype pattern
    /// </summary>
    /// <returns>Copia de la configuración</returns>
    public ConfigurationSystem Clone()
    {
        return new ConfigurationSystem
        {
            VkFilePath = VkFilePath,
            DatFilePath = DatFilePath,
            IdxFilePath = IdxFilePath,
            ImgFileInPath = ImgFileInPath,
            ImgFileOutPath = ImgFileOutPath,
            Enabletamping = Enabletamping,
            EnableIda = EnableIda,
            EnableZipper = EnableZipper,
            EnableConduce = EnableConduce,
            EnableBackup = EnableBackup,
            EnableConduceDummy = EnableConduceDummy,
            EnableVersion = EnableVersion,
            EnableRemoveHeader = EnableRemoveHeader,
            Enable837ToXls = Enable837ToXls,
            ProcessingDate = ProcessingDate,
            TotalImages = TotalImages,
            TotalDocuments = TotalDocuments,
            ConfigurationName = $"{ConfigurationName} - Copy",
            Description = Description,
            CreatedBy = CreatedBy
        };
    }

    #endregion
}
