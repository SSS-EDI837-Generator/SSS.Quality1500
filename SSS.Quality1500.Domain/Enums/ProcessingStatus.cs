namespace SSS.Quality1500.Domain.Enums;

/// <summary>
/// Estados posibles del procesamiento de registros 837P
/// Implementa principio Open/Closed: extensible para nuevos estados
/// </summary>
public enum ProcessingStatus
{
    /// <summary>
    /// El registro está pendiente de procesamiento
    /// </summary>
    Pending = 0,

    /// <summary>
    /// El registro está siendo procesado actualmente
    /// </summary>
    Processing = 1,

    /// <summary>
    /// El procesamiento se completó exitosamente
    /// </summary>
    Completed = 2,

    /// <summary>
    /// El procesamiento falló con errores
    /// </summary>
    Failed = 3,

    /// <summary>
    /// El procesamiento fue cancelado por el usuario
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// El registro está en revisión manual
    /// </summary>
    UnderReview = 5,

    /// <summary>
    /// El registro fue aprobado después de revisión
    /// </summary>
    Approved = 6,

    /// <summary>
    /// El registro fue rechazado después de revisión
    /// </summary>
    Rejected = 7,

    /// <summary>
    /// El registro está en cola para reprocesamiento
    /// </summary>
    QueuedForReprocessing = 8
}

/// <summary>
/// Tipos de archivos del sistema 837P
/// </summary>
public enum FileType837P
{
    /// <summary>
    /// Archivo VK (DBF) - Base de datos de claims
    /// </summary>
    VkFile = 0,

    /// <summary>
    /// Archivo DAT - Datos de configuración
    /// </summary>
    DatFile = 1,

    /// <summary>
    /// Archivo IDX - Índice de registros
    /// </summary>
    IdxFile = 2,

    /// <summary>
    /// Archivo IMG de entrada - Imágenes asociadas
    /// </summary>
    ImgFileIn = 3,

    /// <summary>
    /// Archivo IMG de salida - Imágenes procesadas
    /// </summary>
    ImgFileOut = 4,

    /// <summary>
    /// Archivo LOG - Registro de actividades
    /// </summary>
    LogFile = 5,

    /// <summary>
    /// Archivo de backup
    /// </summary>
    BackupFile = 6
}

/// <summary>
/// Niveles de severidad para logs y notificaciones
/// </summary>
public enum SeverityLevel
{
    /// <summary>
    /// Información general
    /// </summary>
    Info = 0,

    /// <summary>
    /// Advertencia que no impide el procesamiento
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error que impide el procesamiento
    /// </summary>
    Error = 2,

    /// <summary>
    /// Error crítico que requiere atención inmediata
    /// </summary>
    Critical = 3,

    /// <summary>
    /// Depuración (solo en modo desarrollo)
    /// </summary>
    Debug = 4
}

/// <summary>
/// Tipos de operaciones de procesamiento
/// </summary>
public enum ProcessingOperation
{
    /// <summary>
    /// Validación de datos
    /// </summary>
    Validation = 0,

    /// <summary>
    /// Transformación de datos
    /// </summary>
    Transformation = 1,

    /// <summary>
    /// Generación de archivos 837P
    /// </summary>
    FileGeneration = 2,

    /// <summary>
    /// Procesamiento de imágenes
    /// </summary>
    ImageProcessing = 3,

    /// <summary>
    /// Creación de backup
    /// </summary>
    Backup = 4,

    /// <summary>
    /// Limpieza de archivos temporales
    /// </summary>
    Cleanup = 5,

    /// <summary>
    /// Envío de notificaciones
    /// </summary>
    Notification = 6
}
