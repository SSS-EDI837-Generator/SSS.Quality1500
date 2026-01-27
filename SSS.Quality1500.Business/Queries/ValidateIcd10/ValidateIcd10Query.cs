namespace SSS.Quality1500.Business.Queries.ValidateIcd10;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Query to validate an ICD-10 code.
/// </summary>
/// <param name="ColumnName">The DBF column name.</param>
/// <param name="DisplayName">User-friendly field name.</param>
/// <param name="Code">The ICD-10 code to validate.</param>
/// <param name="AllowEmpty">Whether empty values are allowed.</param>
public record ValidateIcd10Query(
    string ColumnName,
    string DisplayName,
    string? Code,
    bool AllowEmpty = false) : IQuery<Result<bool, FieldValidationError>>;
