namespace SSS.Quality1500.Domain.Models;

public record Icd10SearchResult(List<Icd10CodeEntry> Entries, int TotalCount);
