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
