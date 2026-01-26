namespace SSS.Quality1500.Business.Services.Interfaces;

using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Service contract for DBF file validation.
/// </summary>
public interface IDbfValidationService
{
    /// <summary>
    /// Validates a DBF file by checking if all expected columns are present.
    /// </summary>
    /// <param name="filePath">Path to the DBF file.</param>
    /// <returns>Validation result with totals or error information.</returns>
    Task<Result<DbfValidationResult, string>> ValidateDbfFileAsync(string filePath);
}
