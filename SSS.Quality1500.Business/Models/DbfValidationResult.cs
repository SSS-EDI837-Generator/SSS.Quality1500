namespace SSS.Quality1500.Business.Models;

/// <summary>
/// Result of DBF file validation.
/// </summary>
public class DbfValidationResult
{
    /// <summary>
    /// Whether the DBF file is valid (all expected columns present).
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Total number of records in the DBF file (= total images).
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Total number of claims (records where filter column != exclude value).
    /// </summary>
    public int TotalClaims { get; set; }

    /// <summary>
    /// List of missing columns if validation failed.
    /// </summary>
    public List<string> MissingColumns { get; set; } = [];

    /// <summary>
    /// List of actual columns found in the DBF file.
    /// </summary>
    public List<string> ActualColumns { get; set; } = [];

    /// <summary>
    /// Error message if validation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
