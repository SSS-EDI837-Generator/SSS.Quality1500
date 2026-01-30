namespace SSS.Quality1500.Business.Commands.ProcessClaims;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Command to process claims from a DBF file and validate all records.
/// </summary>
/// <param name="DbfFilePath">Path to the DBF file to process.</param>
/// <param name="ImagesFolderPath">Path to the folder containing TIF images.</param>
/// <param name="SelectedColumns">List of column names selected for validation (from configuration).</param>
/// <param name="ValidationPolicies">Validation policies per column (from configuration).</param>
public record ProcessClaimsCommand(
    string DbfFilePath,
    string ImagesFolderPath,
    List<string> SelectedColumns,
    List<ColumnValidationEntry> ValidationPolicies) : ICommand<Result<ClaimProcessingResult, string>>;
