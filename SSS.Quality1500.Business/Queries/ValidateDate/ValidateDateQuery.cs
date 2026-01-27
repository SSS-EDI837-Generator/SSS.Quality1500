namespace SSS.Quality1500.Business.Queries.ValidateDate;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Query to validate a date value.
/// </summary>
/// <param name="ColumnName">The DBF column name.</param>
/// <param name="DisplayName">User-friendly field name.</param>
/// <param name="Value">The date value to validate (can be string, DateTime, or null).</param>
/// <param name="AllowEmpty">Whether empty values are allowed.</param>
/// <param name="AllowFuture">Whether future dates are allowed (default: false).</param>
/// <param name="MinDate">Minimum allowed date (optional).</param>
public record ValidateDateQuery(
    string ColumnName,
    string DisplayName,
    object? Value,
    bool AllowEmpty = false,
    bool AllowFuture = false,
    DateTime? MinDate = null) : IQuery<Result<bool, FieldValidationError>>;
