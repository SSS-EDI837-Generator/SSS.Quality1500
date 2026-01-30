namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Asocia una columna del DBF con su politica de validacion.
/// </summary>
public sealed class ColumnValidationEntry
{
    /// <summary>
    /// Nombre de la columna en el DBF (e.g., "V317BNPI").
    /// </summary>
    public required string ColumnName { get; init; }

    /// <summary>
    /// Politica de validacion asignada a la columna.
    /// </summary>
    public ValidationPolicy Policy { get; init; } = ValidationPolicy.None;
}
