namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

/// <summary>
/// Writes a processing report to disk after each execution.
/// </summary>
public interface IProcessingReportWriter
{
    /// <summary>
    /// Writes a structured report with validation results.
    /// Returns the file path on success.
    /// </summary>
    Result<string, string> WriteReport(ClaimProcessingResult result);
}
