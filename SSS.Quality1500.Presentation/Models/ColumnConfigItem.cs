namespace SSS.Quality1500.Presentation.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using SSS.Quality1500.Domain.Enums;

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
    [NotifyPropertyChangedFor(nameof(IsDateFormatEnabled))]
    [NotifyPropertyChangedFor(nameof(IsAllowedValuesEnabled))]
    private bool _isSelected;

    /// <summary>
    /// Description of the column's purpose.
    /// </summary>
    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>
    /// Tipo de validacion asignada a esta columna.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDateFormatEnabled))]
    [NotifyPropertyChangedFor(nameof(IsAllowedValuesEnabled))]
    private ValidationType _validationType = ValidationType.None;

    /// <summary>
    /// Opciones especificas de la politica de validacion.
    /// </summary>
    [ObservableProperty]
    private Dictionary<string, string> _validationOptions = [];

    /// <summary>
    /// Formato de fecha para columnas con ValidationType.Date.
    /// Se sincroniza con ValidationOptions["DateFormat"].
    /// </summary>
    [ObservableProperty]
    private string _dateFormat = string.Empty;

    /// <summary>
    /// Whether the DateFormat ComboBox should be enabled.
    /// </summary>
    public bool IsDateFormatEnabled => IsSelected && ValidationType == ValidationType.Date;

    partial void OnDateFormatChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            ValidationOptions.Remove("DateFormat");
        else
            ValidationOptions["DateFormat"] = value;
    }

    /// <summary>
    /// Valores permitidos separados por coma (e.g., "0,9").
    /// Se sincroniza con ValidationOptions["AllowedValues"].
    /// </summary>
    [ObservableProperty]
    private string _allowedValues = string.Empty;

    /// <summary>
    /// Whether the AllowedValues TextBox should be enabled.
    /// </summary>
    public bool IsAllowedValuesEnabled => IsSelected && ValidationType == ValidationType.Required;

    partial void OnAllowedValuesChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
            ValidationOptions.Remove("AllowedValues");
        else
            ValidationOptions["AllowedValues"] = value;
    }

    /// <summary>
    /// Formatos de fecha disponibles para seleccion.
    /// </summary>
    public static List<string> AvailableDateFormats { get; } =
    [
        "",
        "MMddyyyy",
        "MMddyy",
        "yyyyMMdd",
        "yyyyMMddHHmmss",
        "MM/dd/yyyy",
        "dd/MM/yyyy",
        "yyyy-MM-dd"
    ];
}
