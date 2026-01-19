
namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;


/// <summary>
/// Interfaz para el servicio de configuración del sistema 837P
/// Implementa principio Dependency Inversion: dependemos de abstracciones
/// Principio Interface Segregation: expone solo métodos relacionados con configuración
/// </summary>
public interface IConfigurationService
{
    #region Configuration Management

    /// <summary>
    /// Obtiene la configuración actual del sistema
    /// </summary>
    /// <returns>Configuración actual o null si no existe</returns>
    Task<ConfigurationSystem?> GetCurrentConfigurationAsync();

    /// <summary>
    /// Guarda una nueva configuración
    /// </summary>
    /// <param name="configuration">Configuración a guardar</param>
    /// <returns>Configuración guardada con ID asignado</returns>
    Task<ConfigurationSystem> SaveConfigurationAsync(ConfigurationSystem configuration);

    /// <summary>
    /// Actualiza una configuración existente
    /// </summary>
    /// <param name="configuration">Configuración a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateConfigurationAsync(ConfigurationSystem configuration);

    /// <summary>
    /// Elimina una configuración (soft delete)
    /// </summary>
    /// <param name="configurationId">ID de la configuración</param>
    /// <param name="deletedBy">Usuario que elimina</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteConfigurationAsync(int configurationId, string deletedBy);

    /// <summary>
    /// Obtiene todas las configuraciones disponibles
    /// </summary>
    /// <param name="includeInactive">Incluir configuraciones inactivas</param>
    /// <returns>Lista de configuraciones</returns>
    Task<IEnumerable<ConfigurationSystem>> GetAllConfigurationsAsync(bool includeInactive = false);

    #endregion

    #region Configuration Validation

    /// <summary>
    /// Valida que una configuración es válida para procesamiento
    /// </summary>
    /// <param name="configuration">Configuración a validar</param>
    /// <returns>Resultado de validación con errores si los hay</returns>
    Task<ValidationResult> ValidateConfigurationAsync(ConfigurationSystem configuration);

    /// <summary>
    /// Verifica que los archivos de configuración existen y son accesibles
    /// </summary>
    /// <param name="configuration">Configuración a verificar</param>
    /// <returns>Resultado de verificación</returns>
    Task<ValidationResult> VerifyConfigurationFilesAsync(ConfigurationSystem configuration);

    #endregion

    #region Batch Management

    /// <summary>
    /// Obtiene los batches asociados a una configuración
    /// </summary>
    /// <param name="configurationId">ID de la configuración</param>
    /// <returns>Lista de batches</returns>
    Task<IEnumerable<BatchRecord>> GetConfigurationBatchesAsync(int configurationId);

    /// <summary>
    /// Agrega batches a una configuración
    /// </summary>
    /// <param name="configurationId">ID de la configuración</param>
    /// <param name="batches">Batches a agregar</param>
    /// <returns>True si se agregaron correctamente</returns>
    Task<bool> AddBatchesToConfigurationAsync(int configurationId, IEnumerable<BatchRecord> batches);

    /// <summary>
    /// Elimina batches de una configuración
    /// </summary>
    /// <param name="configurationId">ID de la configuración</param>
    /// <param name="batchIds">IDs de batches a eliminar</param>
    /// <returns>True si se eliminaron correctamente</returns>
    Task<bool> RemoveBatchesFromConfigurationAsync(int configurationId, IEnumerable<int> batchIds);

    #endregion

    #region Events

    /// <summary>
    /// Evento que se dispara cuando la configuración cambia
    /// </summary>
    event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

    /// <summary>
    /// Evento que se dispara cuando se valida una configuración
    /// </summary>
    event EventHandler<ConfigurationValidatedEventArgs> ConfigurationValidated;

    #endregion
}

/// <summary>
/// Resultado de validación para operaciones de configuración
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Indica si la validación fue exitosa
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Lista de errores encontrados
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Lista de advertencias encontradas
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Crea un resultado de validación exitoso
    /// </summary>
    /// <returns>Resultado válido</returns>
    public static ValidationResult Success() => new() { IsValid = true };

    /// <summary>
    /// Crea un resultado de validación con errores
    /// </summary>
    /// <param name="errors">Lista de errores</param>
    /// <returns>Resultado inválido</returns>
    public static ValidationResult WithErrors(params string[] errors) =>
        new() { IsValid = false, Errors = errors.ToList() };

    /// <summary>
    /// Agrega un error al resultado
    /// </summary>
    /// <param name="error">Error a agregar</param>
    public void AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
    }

    /// <summary>
    /// Agrega una advertencia al resultado
    /// </summary>
    /// <param name="warning">Advertencia a agregar</param>
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}

/// <summary>
/// Argumentos del evento de cambio de configuración
/// </summary>
public class ConfigurationChangedEventArgs : EventArgs
{
    public ConfigurationSystem Configuration { get; }
    public string Operation { get; }
    public DateTime Timestamp { get; }

    public ConfigurationChangedEventArgs(ConfigurationSystem configuration, string operation)
    {
        Configuration = configuration;
        Operation = operation;
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Argumentos del evento de validación de configuración
/// </summary>
public class ConfigurationValidatedEventArgs : EventArgs
{
    public ConfigurationSystem Configuration { get; }
    public ValidationResult ValidationResult { get; }
    public DateTime Timestamp { get; }

    public ConfigurationValidatedEventArgs(ConfigurationSystem configuration, ValidationResult validationResult)
    {
        Configuration = configuration;
        ValidationResult = validationResult;
        Timestamp = DateTime.UtcNow;
    }
}

