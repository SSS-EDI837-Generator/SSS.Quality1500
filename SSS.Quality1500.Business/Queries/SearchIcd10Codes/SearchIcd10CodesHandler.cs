namespace SSS.Quality1500.Business.Queries.SearchIcd10Codes;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

public class SearchIcd10CodesHandler(IIcd10Repository icd10Repository)
    : IQueryHandler<SearchIcd10CodesQuery, Result<Icd10SearchResult, string>>
{
    private readonly IIcd10Repository _icd10Repository = icd10Repository;

    public Task<Result<Icd10SearchResult, string>> HandleAsync(
        SearchIcd10CodesQuery query, CancellationToken ct = default)
    {
        try
        {
            List<Icd10CodeEntry> results = _icd10Repository.SearchCodes(
                query.SearchTerm, query.MaxResults);

            Icd10SearchResult searchResult = new(results, _icd10Repository.TotalCodes);

            return Task.FromResult(Result<Icd10SearchResult, string>.Ok(searchResult));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result<Icd10SearchResult, string>.Fail(
                $"Error al buscar codigos: {ex.Message}"));
        }
    }
}
