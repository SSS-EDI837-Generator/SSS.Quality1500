namespace SSS.Quality1500.Business.Queries.GetImagesFolder;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Query to extract the images folder path (V0FILEPATH) from the first record of a DBF file.
/// </summary>
/// <param name="DbfFilePath">Path to the DBF file.</param>
public record GetImagesFolderQuery(string DbfFilePath) : IQuery<Result<string, string>>;
