namespace SSS.Quality1500.Business.Models;
/// <summary>
/// Resultado del procesamiento
/// </summary>
public record ProcessingResult(
    bool IsSuccess,
    int TotalClaims,
    int TotalServiceLines,
    int FilesGenerated,
    string? ErrorMessage = null,
    List<string>? Warnings = null);