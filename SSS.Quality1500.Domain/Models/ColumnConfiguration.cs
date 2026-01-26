namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Modelo de dominio para la configuracion de columnas a validar.
/// Almacena las columnas seleccionadas por el usuario para verificacion.
/// </summary>
public sealed class ColumnConfiguration
{
    /// <summary>
    /// Version del esquema de configuracion para compatibilidad futura.
    /// </summary>
    public string Version { get; init; } = "1.0";

    /// <summary>
    /// Fecha de ultima modificacion.
    /// </summary>
    public DateTime LastModified { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Lista de nombres de columnas seleccionadas para validacion.
    /// </summary>
    public List<string> SelectedColumns { get; init; } = [];

    /// <summary>
    /// Valida que la configuracion tiene al menos una columna seleccionada.
    /// </summary>
    public bool HasSelectedColumns => SelectedColumns.Count > 0;

    /// <summary>
    /// Crea una configuracion vacia.
    /// </summary>
    public static ColumnConfiguration Empty => new();

    /// <summary>
    /// Crea una configuracion con las columnas especificadas.
    /// </summary>
    public static ColumnConfiguration WithColumns(IEnumerable<string> columns)
    {
        return new ColumnConfiguration
        {
            SelectedColumns = columns.ToList(),
            LastModified = DateTime.UtcNow
        };
    }
}
