namespace SSS.Quality1500.Business.Queries.ValidateDbf;

using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Query to validate a DBF file structure against expected VDE schema.
/// </summary>
/// <param name="FilePath">Path to the DBF file to validate.</param>
public record ValidateDbfQuery(string FilePath) : IQuery<Result<DbfValidationResult, string>>;
