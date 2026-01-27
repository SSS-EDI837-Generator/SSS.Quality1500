namespace SSS.Quality1500.Business.Queries.GetVdeRecords;

using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Query to retrieve VDE records from a DBF file.
/// </summary>
/// <param name="FilePath">Path to the DBF file to read.</param>
public record GetVdeRecordsQuery(string FilePath) : IQuery<Result<List<VdeRecordDto>, string>>;
