namespace SSS.Quality1500.Domain.Entities;

/// <summary>
/// Entidad que representa un registro de claim.
/// Modifique segun las necesidades del proyecto.
/// </summary>
public sealed class ClaimRecord
{
    public string Document { get; init; } = string.Empty;
    public string BatchNumber { get; init; } = string.Empty;
    public int Sequence { get; init; }
    public string CurrentStage { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public int PageNumber { get; init; }

    /// <summary>
    /// Valida que el registro tenga los campos minimos requeridos.
    /// </summary>
    public bool IsValid() => !string.IsNullOrWhiteSpace(Document) && Sequence > 0;
}
