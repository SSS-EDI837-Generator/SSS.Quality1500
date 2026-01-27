namespace SSS.Quality1500.Business.Queries.ValidateDate;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Enums;
using SSS.Quality1500.Domain.Models;
using System.Globalization;

/// <summary>
/// Handler for validating date values.
/// Checks format, future dates, and minimum date constraints.
/// </summary>
public class ValidateDateHandler : IQueryHandler<ValidateDateQuery, Result<bool, FieldValidationError>>
{
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyyMMdd",
        "MM-dd-yyyy"
    ];

    public Task<Result<bool, FieldValidationError>> HandleAsync(
        ValidateDateQuery query, CancellationToken ct = default)
    {
        // Check for empty value
        if (query.Value is null || string.IsNullOrWhiteSpace(query.Value.ToString()))
        {
            if (query.AllowEmpty)
                return Task.FromResult(Result<bool, FieldValidationError>.Ok(true));

            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                FieldValidationError.Required(query.ColumnName, query.DisplayName)));
        }

        // Try to parse the date
        DateTime? parsedDate = ParseDate(query.Value);

        if (parsedDate is null)
        {
            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                FieldValidationError.InvalidFormat(
                    query.ColumnName,
                    query.DisplayName,
                    query.Value,
                    $"Formato de fecha inválido: {query.Value}")));
        }

        DateTime dateValue = parsedDate.Value;

        // Check for future date
        if (!query.AllowFuture && dateValue.Date > DateTime.Today)
        {
            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                FieldValidationError.FutureDate(query.ColumnName, query.DisplayName, query.Value)));
        }

        // Check for minimum date
        if (query.MinDate.HasValue && dateValue.Date < query.MinDate.Value.Date)
        {
            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                new FieldValidationError
                {
                    ColumnName = query.ColumnName,
                    DisplayName = query.DisplayName,
                    CurrentValue = query.Value,
                    ErrorMessage = $"La fecha {dateValue:yyyy-MM-dd} es anterior al mínimo permitido ({query.MinDate.Value:yyyy-MM-dd}).",
                    ErrorType = ValidationErrorType.DateTooOld
                }));
        }

        return Task.FromResult(Result<bool, FieldValidationError>.Ok(true));
    }

    private static DateTime? ParseDate(object? value)
    {
        if (value is DateTime dt)
            return dt;

        if (value is not string stringValue)
            return null;

        string trimmed = stringValue.Trim();

        if (string.IsNullOrEmpty(trimmed))
            return null;

        // Try standard formats
        if (DateTime.TryParseExact(trimmed, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            return result;

        // Try general parsing as fallback
        if (DateTime.TryParse(trimmed, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return result;

        return null;
    }
}
