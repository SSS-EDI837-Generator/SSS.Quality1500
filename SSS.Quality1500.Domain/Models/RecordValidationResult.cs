namespace SSS.Quality1500.Domain.Models;

/// <summary>
/// Represents the validation result for a single record in the DBF file.
/// </summary>
public sealed class RecordValidationResult
{
    /// <summary>
    /// The zero-based index of the record in the DBF file.
    /// </summary>
    public required int RecordIndex { get; init; }

    /// <summary>
    /// The filename of the associated TIF image (from V0IFNAME01 column).
    /// </summary>
    public string ImageFileName { get; init; } = string.Empty;

    /// <summary>
    /// All field values from the record, keyed by column name.
    /// </summary>
    public Dictionary<string, object?> RecordData { get; init; } = [];

    /// <summary>
    /// List of validation errors found in this record.
    /// </summary>
    public List<FieldValidationError> FieldErrors { get; init; } = [];

    /// <summary>
    /// Whether the record has any validation errors.
    /// </summary>
    public bool HasErrors => FieldErrors.Count > 0;

    /// <summary>
    /// Total number of errors in this record.
    /// </summary>
    public int ErrorCount => FieldErrors.Count;

    /// <summary>
    /// Creates a record validation result with no errors.
    /// </summary>
    public static RecordValidationResult Valid(int recordIndex, string imageFileName, Dictionary<string, object?> recordData) =>
        new()
        {
            RecordIndex = recordIndex,
            ImageFileName = imageFileName,
            RecordData = recordData,
            FieldErrors = []
        };

    /// <summary>
    /// Creates a record validation result with errors.
    /// </summary>
    public static RecordValidationResult WithErrors(
        int recordIndex,
        string imageFileName,
        Dictionary<string, object?> recordData,
        List<FieldValidationError> errors) =>
        new()
        {
            RecordIndex = recordIndex,
            ImageFileName = imageFileName,
            RecordData = recordData,
            FieldErrors = errors
        };
}
