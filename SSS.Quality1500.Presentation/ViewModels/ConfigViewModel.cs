namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSS.Quality1500.Presentation.Models;
using System.Collections.ObjectModel;

/// <summary>
/// ViewModel for the column configuration view.
/// Allows users to select which DBF columns to verify during processing.
/// </summary>
public partial class ConfigViewModel : ObservableObject
{
    [ObservableProperty]
    private string _pageTitle = "Configuracion de Columnas";

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Todas";

    [ObservableProperty]
    private ObservableCollection<ColumnConfigItem> _allColumns = [];

    [ObservableProperty]
    private ObservableCollection<ColumnConfigItem> _filteredColumns = [];

    [ObservableProperty]
    private ObservableCollection<string> _categories = [];

    [ObservableProperty]
    private int _selectedCount;

    [ObservableProperty]
    private int _totalCount;

    public ConfigViewModel()
    {
        LoadColumns();
        UpdateFilteredColumns();
    }

    partial void OnSearchTextChanged(string value)
    {
        UpdateFilteredColumns();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        UpdateFilteredColumns();
    }

    private void LoadColumns()
    {
        Categories =
        [
            "Todas",
            "Documento",
            "Paciente",
            "Asegurado",
            "Proveedor Referidor",
            "Diagnosticos",
            "Fechas",
            "Montos",
            "Facilidad",
            "Proveedor Facturacion",
            "Lineas de Servicio"
        ];

        AllColumns =
        [
            // Document Section (V0*)
            new ColumnConfigItem { ColumnName = "V0DOCUMENT", DisplayName = "Numero Documento", BoxReference = "N/A", Category = "Documento", Description = "Identificador unico del documento" },
            new ColumnConfigItem { ColumnName = "V0BATCHNUM", DisplayName = "Numero de Batch", BoxReference = "N/A", Category = "Documento", Description = "Numero del lote de procesamiento" },
            new ColumnConfigItem { ColumnName = "V0SEQUENCE", DisplayName = "Secuencia", BoxReference = "N/A", Category = "Documento", Description = "Numero de secuencia en el batch" },
            new ColumnConfigItem { ColumnName = "V0IFNAME01", DisplayName = "Nombre Archivo Imagen", BoxReference = "N/A", Category = "Documento", Description = "Ruta del archivo de imagen asociado" },

            // Patient Section (V1* - Box 1-4)
            new ColumnConfigItem { ColumnName = "V11AINSURE", DisplayName = "ID Asegurado", BoxReference = "Box 1a", Category = "Paciente", Description = "Numero de identificacion del asegurado", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V11TYPEID", DisplayName = "Tipo de Cobertura", BoxReference = "Box 1", Category = "Paciente", Description = "Medicare/Medicaid/Group/etc" },
            new ColumnConfigItem { ColumnName = "V12NAME", DisplayName = "Nombre Paciente", BoxReference = "Box 2", Category = "Paciente", Description = "Nombre del paciente" },
            new ColumnConfigItem { ColumnName = "V12LASTNAM", DisplayName = "Apellido Paciente", BoxReference = "Box 2", Category = "Paciente", Description = "Apellido del paciente" },
            new ColumnConfigItem { ColumnName = "V12INITIAL", DisplayName = "Inicial Paciente", BoxReference = "Box 2", Category = "Paciente", Description = "Inicial del segundo nombre" },
            new ColumnConfigItem { ColumnName = "V13BIRTH", DisplayName = "Fecha Nacimiento", BoxReference = "Box 3", Category = "Paciente", Description = "Fecha de nacimiento del paciente", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V13SEXO", DisplayName = "Sexo Paciente", BoxReference = "Box 3", Category = "Paciente", Description = "M/F" },

            // Patient Address (V1* - Box 5)
            new ColumnConfigItem { ColumnName = "V15ADDRES1", DisplayName = "Direccion 1", BoxReference = "Box 5", Category = "Paciente", Description = "Direccion linea 1" },
            new ColumnConfigItem { ColumnName = "V15ADDRES2", DisplayName = "Direccion 2", BoxReference = "Box 5", Category = "Paciente", Description = "Direccion linea 2" },
            new ColumnConfigItem { ColumnName = "V15CITY", DisplayName = "Ciudad", BoxReference = "Box 5", Category = "Paciente", Description = "Ciudad del paciente" },
            new ColumnConfigItem { ColumnName = "V15STATE", DisplayName = "Estado", BoxReference = "Box 5", Category = "Paciente", Description = "Estado del paciente" },
            new ColumnConfigItem { ColumnName = "V15ZIPCODE", DisplayName = "Codigo Postal", BoxReference = "Box 5", Category = "Paciente", Description = "ZIP Code" },
            new ColumnConfigItem { ColumnName = "V15PLUS4", DisplayName = "ZIP+4", BoxReference = "Box 5", Category = "Paciente", Description = "Extension ZIP+4" },
            new ColumnConfigItem { ColumnName = "V15TELEFON", DisplayName = "Telefono Paciente", BoxReference = "Box 5", Category = "Paciente", Description = "Numero de telefono" },

            // Insured Information (V2* - Box 9, 11)
            new ColumnConfigItem { ColumnName = "V29NAME", DisplayName = "Nombre Otro Asegurado", BoxReference = "Box 9", Category = "Asegurado", Description = "Nombre si hay otro seguro" },
            new ColumnConfigItem { ColumnName = "V29LASTNAM", DisplayName = "Apellido Otro Asegurado", BoxReference = "Box 9", Category = "Asegurado", Description = "Apellido si hay otro seguro" },
            new ColumnConfigItem { ColumnName = "V29APOLICY", DisplayName = "Poliza Otro Asegurado", BoxReference = "Box 9a", Category = "Asegurado", Description = "Numero de poliza" },
            new ColumnConfigItem { ColumnName = "V29DINSPLA", DisplayName = "Plan de Seguro", BoxReference = "Box 9d", Category = "Asegurado", Description = "Nombre del plan de seguro" },
            new ColumnConfigItem { ColumnName = "V210AEMPLO", DisplayName = "Es por Empleo", BoxReference = "Box 10a", Category = "Asegurado", Description = "Condicion relacionada a empleo" },
            new ColumnConfigItem { ColumnName = "V210BAUTO", DisplayName = "Accidente Auto", BoxReference = "Box 10b", Category = "Asegurado", Description = "Condicion por accidente auto" },
            new ColumnConfigItem { ColumnName = "V210COTHER", DisplayName = "Otro Accidente", BoxReference = "Box 10c", Category = "Asegurado", Description = "Otra condicion de accidente" },
            new ColumnConfigItem { ColumnName = "V211ABIRTH", DisplayName = "Fecha Nac. Asegurado", BoxReference = "Box 11a", Category = "Asegurado", Description = "Fecha nacimiento del asegurado" },
            new ColumnConfigItem { ColumnName = "V211ASEXO", DisplayName = "Sexo Asegurado", BoxReference = "Box 11a", Category = "Asegurado", Description = "Sexo del asegurado" },
            new ColumnConfigItem { ColumnName = "V211INSURE", DisplayName = "Nombre Plan Asegurado", BoxReference = "Box 11c", Category = "Asegurado", Description = "Nombre del plan de seguro" },

            // Referring Provider (V3* - Box 17)
            new ColumnConfigItem { ColumnName = "V317NAME", DisplayName = "Nombre Proveedor Ref.", BoxReference = "Box 17", Category = "Proveedor Referidor", Description = "Nombre del proveedor que refiere" },
            new ColumnConfigItem { ColumnName = "V317AQUAL", DisplayName = "Qualifier 17a", BoxReference = "Box 17a", Category = "Proveedor Referidor", Description = "Calificador de ID" },
            new ColumnConfigItem { ColumnName = "V317AREFFE", DisplayName = "ID Referidor", BoxReference = "Box 17a", Category = "Proveedor Referidor", Description = "Numero ID del referidor" },
            new ColumnConfigItem { ColumnName = "V317BNPI", DisplayName = "NPI Referidor", BoxReference = "Box 17b", Category = "Proveedor Referidor", Description = "NPI del proveedor que refiere", IsSelected = true },

            // Dates (V3* - Box 14-18)
            new ColumnConfigItem { ColumnName = "V314DATE", DisplayName = "Fecha Condicion Actual", BoxReference = "Box 14", Category = "Fechas", Description = "Fecha inicio condicion actual", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V314QUAL", DisplayName = "Qualifier Fecha 14", BoxReference = "Box 14", Category = "Fechas", Description = "431=Onset, 484=Initial Treatment" },
            new ColumnConfigItem { ColumnName = "V315DATE", DisplayName = "Fecha Similar", BoxReference = "Box 15", Category = "Fechas", Description = "Fecha condicion similar" },
            new ColumnConfigItem { ColumnName = "V315QUAL", DisplayName = "Qualifier Fecha 15", BoxReference = "Box 15", Category = "Fechas", Description = "Calificador fecha Box 15" },
            new ColumnConfigItem { ColumnName = "V316DATEFR", DisplayName = "Fecha Desde Incapacidad", BoxReference = "Box 16", Category = "Fechas", Description = "Fecha desde no puede trabajar", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V316DATETO", DisplayName = "Fecha Hasta Incapacidad", BoxReference = "Box 16", Category = "Fechas", Description = "Fecha hasta no puede trabajar", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V318DATEFR", DisplayName = "Fecha Admision Hospital", BoxReference = "Box 18", Category = "Fechas", Description = "Fecha de admision" },
            new ColumnConfigItem { ColumnName = "V318DATETO", DisplayName = "Fecha Alta Hospital", BoxReference = "Box 18", Category = "Fechas", Description = "Fecha de alta" },

            // Diagnosis (V3/V4* - Box 21)
            new ColumnConfigItem { ColumnName = "V321ICDIND", DisplayName = "Indicador ICD", BoxReference = "Box 21", Category = "Diagnosticos", Description = "9=ICD-9, 0=ICD-10", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V321DIAG", DisplayName = "Codigos Diagnostico", BoxReference = "Box 21", Category = "Diagnosticos", Description = "Codigos de diagnostico A-L", IsSelected = true },

            // Amounts (V4* - Box 28-30)
            new ColumnConfigItem { ColumnName = "V428TOTAL", DisplayName = "Total Cargos", BoxReference = "Box 28", Category = "Montos", Description = "Total de cargos", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V429AMOUNT", DisplayName = "Monto Pagado", BoxReference = "Box 29", Category = "Montos", Description = "Monto ya pagado" },
            new ColumnConfigItem { ColumnName = "V430NUCC", DisplayName = "Balance Due NUCC", BoxReference = "Box 30", Category = "Montos", Description = "Balance pendiente" },

            // Facility (V4* - Box 32)
            new ColumnConfigItem { ColumnName = "V432ANPI", DisplayName = "NPI Facilidad", BoxReference = "Box 32a", Category = "Facilidad", Description = "NPI de la facilidad", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V432BOTHER", DisplayName = "Otro ID Facilidad", BoxReference = "Box 32b", Category = "Facilidad", Description = "Otro identificador" },
            new ColumnConfigItem { ColumnName = "V432BQUAL", DisplayName = "Qualifier Facilidad", BoxReference = "Box 32b", Category = "Facilidad", Description = "Calificador otro ID" },
            new ColumnConfigItem { ColumnName = "V432NAME", DisplayName = "Nombre Facilidad", BoxReference = "Box 32", Category = "Facilidad", Description = "Nombre de la facilidad" },
            new ColumnConfigItem { ColumnName = "V432ADDRE1", DisplayName = "Direccion Facilidad", BoxReference = "Box 32", Category = "Facilidad", Description = "Direccion linea 1" },
            new ColumnConfigItem { ColumnName = "V432CITY", DisplayName = "Ciudad Facilidad", BoxReference = "Box 32", Category = "Facilidad", Description = "Ciudad de la facilidad" },
            new ColumnConfigItem { ColumnName = "V432STATE", DisplayName = "Estado Facilidad", BoxReference = "Box 32", Category = "Facilidad", Description = "Estado de la facilidad" },
            new ColumnConfigItem { ColumnName = "V432ZIPCOD", DisplayName = "ZIP Facilidad", BoxReference = "Box 32", Category = "Facilidad", Description = "Codigo postal facilidad" },

            // Billing Provider (V4* - Box 33)
            new ColumnConfigItem { ColumnName = "V433ANPI", DisplayName = "NPI Facturacion", BoxReference = "Box 33a", Category = "Proveedor Facturacion", Description = "NPI del proveedor de facturacion", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V433BTAXON", DisplayName = "Taxonomy Facturacion", BoxReference = "Box 33b", Category = "Proveedor Facturacion", Description = "Codigo taxonomy" },
            new ColumnConfigItem { ColumnName = "V433BQUAL", DisplayName = "Qualifier Facturacion", BoxReference = "Box 33b", Category = "Proveedor Facturacion", Description = "Calificador ID" },
            new ColumnConfigItem { ColumnName = "V433NAME", DisplayName = "Nombre Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Nombre del proveedor" },
            new ColumnConfigItem { ColumnName = "V433LASTNA", DisplayName = "Apellido Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Apellido del proveedor" },
            new ColumnConfigItem { ColumnName = "V433ADDRE1", DisplayName = "Direccion Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Direccion linea 1" },
            new ColumnConfigItem { ColumnName = "V433CITY", DisplayName = "Ciudad Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Ciudad del proveedor" },
            new ColumnConfigItem { ColumnName = "V433STATE", DisplayName = "Estado Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Estado del proveedor" },
            new ColumnConfigItem { ColumnName = "V433ZIPCOD", DisplayName = "ZIP Facturacion", BoxReference = "Box 33", Category = "Proveedor Facturacion", Description = "Codigo postal" },

            // Service Lines (Box 24 - Line 1 example, V5*)
            new ColumnConfigItem { ColumnName = "V524ADATEF", DisplayName = "L1: Fecha Desde", BoxReference = "Box 24A", Category = "Lineas de Servicio", Description = "Fecha desde linea 1", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524ADATET", DisplayName = "L1: Fecha Hasta", BoxReference = "Box 24A", Category = "Lineas de Servicio", Description = "Fecha hasta linea 1", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524BPLACE", DisplayName = "L1: Place of Service", BoxReference = "Box 24B", Category = "Lineas de Servicio", Description = "Codigo lugar de servicio", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524CEMG", DisplayName = "L1: Emergencia", BoxReference = "Box 24C", Category = "Lineas de Servicio", Description = "Indicador emergencia" },
            new ColumnConfigItem { ColumnName = "V524DCPT", DisplayName = "L1: CPT/HCPCS", BoxReference = "Box 24D", Category = "Lineas de Servicio", Description = "Codigo de procedimiento", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524DMOD1", DisplayName = "L1: Modificador 1", BoxReference = "Box 24D", Category = "Lineas de Servicio", Description = "Primer modificador" },
            new ColumnConfigItem { ColumnName = "V524DMOD2", DisplayName = "L1: Modificador 2", BoxReference = "Box 24D", Category = "Lineas de Servicio", Description = "Segundo modificador" },
            new ColumnConfigItem { ColumnName = "V524DMOD3", DisplayName = "L1: Modificador 3", BoxReference = "Box 24D", Category = "Lineas de Servicio", Description = "Tercer modificador" },
            new ColumnConfigItem { ColumnName = "V524DMOD4", DisplayName = "L1: Modificador 4", BoxReference = "Box 24D", Category = "Lineas de Servicio", Description = "Cuarto modificador" },
            new ColumnConfigItem { ColumnName = "V524EDIAGN", DisplayName = "L1: Pointer Diagnostico", BoxReference = "Box 24E", Category = "Lineas de Servicio", Description = "Apuntador a diagnosticos", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524FCHARG", DisplayName = "L1: Cargos", BoxReference = "Box 24F", Category = "Lineas de Servicio", Description = "Monto del cargo", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524GDAYS", DisplayName = "L1: Dias/Unidades", BoxReference = "Box 24G", Category = "Lineas de Servicio", Description = "Cantidad de unidades", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524JNPI", DisplayName = "L1: Rendering NPI", BoxReference = "Box 24J", Category = "Lineas de Servicio", Description = "NPI del proveedor que rindio servicio", IsSelected = true },
            new ColumnConfigItem { ColumnName = "V524JTAXON", DisplayName = "L1: Rendering Taxonomy", BoxReference = "Box 24J", Category = "Lineas de Servicio", Description = "Taxonomy del rendering" },
            new ColumnConfigItem { ColumnName = "V524NDC", DisplayName = "L1: NDC Code", BoxReference = "Box 24", Category = "Lineas de Servicio", Description = "National Drug Code" },
        ];

        TotalCount = AllColumns.Count;
        UpdateSelectedCount();

        // Subscribe to changes
        foreach (ColumnConfigItem column in AllColumns)
        {
            column.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ColumnConfigItem.IsSelected))
                {
                    UpdateSelectedCount();
                }
            };
        }
    }

    private void UpdateFilteredColumns()
    {
        IEnumerable<ColumnConfigItem> filtered = AllColumns;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            string search = SearchText.ToUpperInvariant();
            filtered = filtered.Where(c =>
                c.ColumnName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.BoxReference.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (SelectedCategory != "Todas")
        {
            filtered = filtered.Where(c => c.Category == SelectedCategory);
        }

        FilteredColumns = new ObservableCollection<ColumnConfigItem>(filtered);
    }

    private void UpdateSelectedCount()
    {
        SelectedCount = AllColumns.Count(c => c.IsSelected);
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (ColumnConfigItem column in FilteredColumns)
        {
            column.IsSelected = true;
        }
    }

    [RelayCommand]
    private void DeselectAll()
    {
        foreach (ColumnConfigItem column in FilteredColumns)
        {
            column.IsSelected = false;
        }
    }

    [RelayCommand]
    private void SaveConfiguration()
    {
        List<string> selectedColumns = AllColumns
            .Where(c => c.IsSelected)
            .Select(c => c.ColumnName)
            .ToList();

        // TODO: Save to configuration file or database
        System.Diagnostics.Debug.WriteLine($"Saved {selectedColumns.Count} columns for validation");
    }
}
