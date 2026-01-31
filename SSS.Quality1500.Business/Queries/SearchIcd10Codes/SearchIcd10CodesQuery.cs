namespace SSS.Quality1500.Business.Queries.SearchIcd10Codes;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

public record SearchIcd10CodesQuery(string SearchTerm, int MaxResults = 100)
    : IQuery<Result<Icd10SearchResult, string>>;
