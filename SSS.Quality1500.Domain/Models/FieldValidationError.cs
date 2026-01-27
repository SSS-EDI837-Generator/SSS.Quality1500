namespace SSS.Quality1500.Domain.Models;

using SSS.Quality1500.Domain.Enums;

/// <summary>
/// Represents a validation error for a specific field in a record.
/// </summary>
public sealed class FieldValidationError
{
    /// <summary>
    /// The column name in the DBF file (e.g., "V0NPIPROV").
    /// </summary>
    public required string ColumnName { get; init; }

    /// <summary>
    /// User-friendly display name for the field (e.g., "NPI Proveedor").
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// The current value that failed validation.
    /// </summary>
    public object? CurrentValue { get; init; }

    /// <summary>
    /// Human-readable error message describing the problem.
    /// </summary>
    public required string ErrorMessage { get; init; }

    /// <summary>
    /// The type of validation error.
    /// </summary>
    public required ValidationErrorType ErrorType { get; init; }

    /// <summary>
    /// Creates a field validation error for an invalid format.
    /// </summary>
    public static FieldValidationError InvalidFormat(string columnName, string displayName, object? value, string message) =>
        new()
        {
            ColumnName = columnName,
            DisplayName = displayName,
            CurrentValue = value,
            ErrorMessage = message,
            ErrorType = ValidationErrorType.InvalidFormat
        };

    /// <summary>
    /// Creates a field validation error for a future date.
    /// </summary>
    public static FieldValidationError FutureDate(string columnName, string displayName, object? value) =>
        new()
        {
            ColumnName = columnName,
            DisplayName = displayName,
            CurrentValue = value,
            ErrorMessage = "La fecha no puede ser futura.",
            ErrorType = ValidationErrorType.FutureDate
        };

    /// <summary>
    /// Creates a field validation error for an invalid code.
    /// </summary>
    public static FieldValidationError InvalidCode(string columnName, string displayName, object? value, string codeType) =>
        new()
        {
            ColumnName = columnName,
            DisplayName = displayName,
            CurrentValue = value,
            ErrorMessage = $"Código {codeType} inválido: {value}",
            ErrorType = ValidationErrorType.InvalidCode
        };

    /// <summary>
    /// Creates a field validation error for a value not found.
    /// </summary>
    public static FieldValidationError NotFound(string columnName, string displayName, object? value, string source) =>
        new()
        {
            ColumnName = columnName,
            DisplayName = displayName,
            CurrentValue = value,
            ErrorMessage = $"No encontrado en {source}: {value}",
            ErrorType = ValidationErrorType.NotFound
        };

    /// <summary>
    /// Creates a field validation error for a required field.
    /// </summary>
    public static FieldValidationError Required(string columnName, string displayName) =>
        new()
        {
            ColumnName = columnName,
            DisplayName = displayName,
            CurrentValue = null,
            ErrorMessage = "Este campo es requerido.",
            ErrorType = ValidationErrorType.Required
        };
}
