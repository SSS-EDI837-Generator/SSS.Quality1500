namespace SSS.Quality1500.Business.Models;

/// <summary>
/// Settings for DBF validation and claim filtering.
/// Loaded from appsettings.json.
/// </summary>
public class DbfValidationSettings
{
    /// <summary>
    /// Configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "DbfValidation";

    /// <summary>
    /// Default folder path for DBF database files.
    /// Loaded on startup as the initial SelectedPath.
    /// </summary>
    public string DefaultDatabasePath { get; set; } = string.Empty;

    /// <summary>
    /// File filter pattern for DBF files in the selected folder.
    /// Default: "*.DBF" (all DBF files).
    /// Example: "SSS15*.DBF" (only SSS 1500 files).
    /// </summary>
    public string FileFilterPattern { get; set; } = "*.DBF";

    /// <summary>
    /// Column name used to filter claims from total records.
    /// Default: "V1PAGINA"
    /// </summary>
    public string ClaimFilterColumn { get; set; } = "V1PAGINA";

    /// <summary>
    /// Value to exclude when counting claims.
    /// Records where ClaimFilterColumn != ClaimFilterExcludeValue are counted as claims.
    /// Default: "99"
    /// </summary>
    public string ClaimFilterExcludeValue { get; set; } = "99";
}
