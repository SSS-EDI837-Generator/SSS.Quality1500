namespace SSS.Quality1500.Presentation.Models;

using CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// Represents a configurable column for VDE record validation.
/// Used in ConfigUserControl for selecting which columns to verify.
/// </summary>
public partial class ColumnConfigItem : ObservableObject
{
    /// <summary>
    /// The DBF column name (e.g., "V317BNPI").
    /// </summary>
    [ObservableProperty]
    private string _columnName = string.Empty;

    /// <summary>
    /// User-friendly display name (e.g., "Referring Provider NPI").
    /// </summary>
    [ObservableProperty]
    private string _displayName = string.Empty;

    /// <summary>
    /// CMS-1500 box reference (e.g., "Box 17b").
    /// </summary>
    [ObservableProperty]
    private string _boxReference = string.Empty;

    /// <summary>
    /// Category for grouping (e.g., "Provider", "Patient", "Dates").
    /// </summary>
    [ObservableProperty]
    private string _category = string.Empty;

    /// <summary>
    /// Whether this column is selected for validation.
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Description of the column's purpose.
    /// </summary>
    [ObservableProperty]
    private string _description = string.Empty;
}
