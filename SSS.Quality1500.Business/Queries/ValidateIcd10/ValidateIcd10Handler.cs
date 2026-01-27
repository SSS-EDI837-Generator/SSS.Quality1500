namespace SSS.Quality1500.Business.Queries.ValidateIcd10;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Handler for validating ICD-10 codes against the local repository.
/// </summary>
public class ValidateIcd10Handler(IIcd10Repository icd10Repository)
    : IQueryHandler<ValidateIcd10Query, Result<bool, FieldValidationError>>
{
    private readonly IIcd10Repository _icd10Repository = icd10Repository;

    public Task<Result<bool, FieldValidationError>> HandleAsync(
        ValidateIcd10Query query, CancellationToken ct = default)
    {
        // Check for empty value
        if (string.IsNullOrWhiteSpace(query.Code))
        {
            if (query.AllowEmpty)
                return Task.FromResult(Result<bool, FieldValidationError>.Ok(true));

            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                FieldValidationError.Required(query.ColumnName, query.DisplayName)));
        }

        // Validate against repository
        if (!_icd10Repository.IsValidCode(query.Code))
        {
            return Task.FromResult(Result<bool, FieldValidationError>.Fail(
                FieldValidationError.InvalidCode(query.ColumnName, query.DisplayName, query.Code, "ICD-10")));
        }

        return Task.FromResult(Result<bool, FieldValidationError>.Ok(true));
    }
}
