namespace SSS.Quality1500.Domain.Enums;

/// <summary>
/// Types of validation errors that can occur during claim processing.
/// </summary>
public enum ValidationErrorType
{
    /// <summary>
    /// The value format is invalid (e.g., date format, numeric format).
    /// </summary>
    InvalidFormat,

    /// <summary>
    /// The value was not found in the validation source (e.g., NPI not in registry).
    /// </summary>
    NotFound,

    /// <summary>
    /// The date is in the future, which is not allowed.
    /// </summary>
    FutureDate,

    /// <summary>
    /// The code is invalid (e.g., ICD-10, CPT code not recognized).
    /// </summary>
    InvalidCode,

    /// <summary>
    /// A required field is missing or empty.
    /// </summary>
    Required,

    /// <summary>
    /// The value is outside the acceptable range.
    /// </summary>
    OutOfRange,

    /// <summary>
    /// The date is too old to be valid.
    /// </summary>
    DateTooOld,

    /// <summary>
    /// API validation failed or was unavailable.
    /// </summary>
    ApiValidationFailed
}
