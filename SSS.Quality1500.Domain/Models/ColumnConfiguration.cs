namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Modelo de dominio para la configuracion de columnas a validar.
/// Almacena las columnas seleccionadas y sus politicas de validacion.
/// </summary>
public sealed class ColumnConfiguration
{
    /// <summary>
    /// Version del esquema de configuracion para compatibilidad futura.
    /// </summary>
    public string Version { get; init; } = "2.0";

    /// <summary>
    /// Fecha de ultima modificacion.
    /// </summary>
    public DateTime LastModified { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Lista de nombres de columnas seleccionadas para validacion.
    /// </summary>
    public List<string> SelectedColumns { get; init; } = [];

    /// <summary>
    /// Politicas de validacion por columna.
    /// </summary>
    public List<ColumnValidationEntry> ValidationPolicies { get; init; } = [];

    /// <summary>
    /// Valida que la configuracion tiene al menos una columna seleccionada.
    /// </summary>
    public bool HasSelectedColumns => SelectedColumns.Count > 0;

    /// <summary>
    /// Valida que la configuracion tiene al menos una politica de validacion.
    /// </summary>
    public bool HasValidationPolicies => ValidationPolicies.Count > 0;

    /// <summary>
    /// Crea una configuracion vacia.
    /// </summary>
    public static ColumnConfiguration Empty => new();

    /// <summary>
    /// Obtiene la politica de validacion para una columna especifica.
    /// Retorna ValidationPolicy.None si no tiene politica asignada.
    /// </summary>
    public ValidationPolicy GetPolicy(string columnName)
    {
        ColumnValidationEntry? entry = ValidationPolicies
            .FirstOrDefault(e => string.Equals(e.ColumnName, columnName, StringComparison.Ordinal));

        if (entry is null)
            return ValidationPolicy.None;

        return entry.Policy;
    }

    /// <summary>
    /// Crea una configuracion con las columnas especificadas (sin politicas).
    /// </summary>
    public static ColumnConfiguration WithColumns(IEnumerable<string> columns)
    {
        return new ColumnConfiguration
        {
            SelectedColumns = columns.ToList(),
            LastModified = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Crea una configuracion con columnas y politicas de validacion.
    /// </summary>
    public static ColumnConfiguration WithPolicies(IEnumerable<ColumnValidationEntry> entries)
    {
        List<ColumnValidationEntry> policies = entries.ToList();
        return new ColumnConfiguration
        {
            SelectedColumns = policies.Select(e => e.ColumnName).ToList(),
            ValidationPolicies = policies,
            LastModified = DateTime.UtcNow
        };
    }
}
