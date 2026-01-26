namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

/// <summary>
/// Contrato para persistencia de configuracion de columnas.
/// Define operaciones de carga y guardado sin especificar implementacion.
/// </summary>
public interface IColumnConfigurationRepository
{
    /// <summary>
    /// Carga la configuracion de columnas desde el almacenamiento.
    /// </summary>
    /// <returns>Result con la configuracion o mensaje de error.</returns>
    Result<ColumnConfiguration, string> Load();

    /// <summary>
    /// Guarda la configuracion de columnas en el almacenamiento.
    /// </summary>
    /// <param name="configuration">Configuracion a guardar.</param>
    /// <returns>Result con true si se guardo correctamente o mensaje de error.</returns>
    Result<bool, string> Save(ColumnConfiguration configuration);

    /// <summary>
    /// Verifica si existe una configuracion guardada.
    /// </summary>
    bool Exists();
}
