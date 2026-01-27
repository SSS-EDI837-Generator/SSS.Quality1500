namespace SSS.Quality1500.Business.Commands.ProcessClaims;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Command to process claims from a DBF file and validate all records.
/// </summary>
/// <param name="DbfFilePath">Path to the DBF file to process.</param>
/// <param name="ImagesFolderPath">Path to the folder containing TIF images.</param>
/// <param name="SelectedColumns">List of column names selected for validation (from configuration).</param>
/// <param name="DateColumns">Column names that contain dates to validate.</param>
/// <param name="Icd10Columns">Column names that contain ICD-10 codes to validate.</param>
public record ProcessClaimsCommand(
    string DbfFilePath,
    string ImagesFolderPath,
    List<string> SelectedColumns,
    List<string> DateColumns,
    List<string> Icd10Columns) : ICommand<Result<ClaimProcessingResult, string>>;
