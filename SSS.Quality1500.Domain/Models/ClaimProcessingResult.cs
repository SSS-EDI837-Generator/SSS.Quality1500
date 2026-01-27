namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Represents the complete result of processing a DBF file with claim records.
/// </summary>
public sealed class ClaimProcessingResult
{
    /// <summary>
    /// Total number of records processed.
    /// </summary>
    public int TotalRecords { get; init; }

    /// <summary>
    /// Number of records that passed all validations.
    /// </summary>
    public int ValidRecords { get; init; }

    /// <summary>
    /// Number of records that have at least one validation error.
    /// </summary>
    public int RecordsWithErrors { get; init; }

    /// <summary>
    /// Total number of individual field errors across all records.
    /// </summary>
    public int TotalFieldErrors { get; init; }

    /// <summary>
    /// List of records that have validation errors.
    /// Only records with errors are included to save memory.
    /// </summary>
    public List<RecordValidationResult> ErrorRecords { get; init; } = [];

    /// <summary>
    /// Timestamp when processing was completed.
    /// </summary>
    public DateTime ProcessedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Path to the DBF file that was processed.
    /// </summary>
    public string SourceFilePath { get; init; } = string.Empty;

    /// <summary>
    /// Path to the images folder used during processing.
    /// </summary>
    public string ImagesFolderPath { get; init; } = string.Empty;

    /// <summary>
    /// Whether all records passed validation.
    /// </summary>
    public bool AllRecordsValid => RecordsWithErrors == 0;

    /// <summary>
    /// Percentage of records that passed validation.
    /// </summary>
    public double SuccessRate => TotalRecords > 0
        ? Math.Round((double)ValidRecords / TotalRecords * 100, 2)
        : 0;

    /// <summary>
    /// Creates an empty result for when processing hasn't started.
    /// </summary>
    public static ClaimProcessingResult Empty => new()
    {
        TotalRecords = 0,
        ValidRecords = 0,
        RecordsWithErrors = 0,
        TotalFieldErrors = 0,
        ErrorRecords = []
    };

    /// <summary>
    /// Creates a processing result from a list of record validation results.
    /// </summary>
    public static ClaimProcessingResult FromValidationResults(
        string sourceFilePath,
        string imagesFolderPath,
        List<RecordValidationResult> allResults)
    {
        List<RecordValidationResult> errorRecords = allResults
            .Where(r => r.HasErrors)
            .ToList();

        int totalFieldErrors = errorRecords.Sum(r => r.ErrorCount);

        return new ClaimProcessingResult
        {
            SourceFilePath = sourceFilePath,
            ImagesFolderPath = imagesFolderPath,
            TotalRecords = allResults.Count,
            ValidRecords = allResults.Count - errorRecords.Count,
            RecordsWithErrors = errorRecords.Count,
            TotalFieldErrors = totalFieldErrors,
            ErrorRecords = errorRecords,
            ProcessedAt = DateTime.UtcNow
        };
    }
}
