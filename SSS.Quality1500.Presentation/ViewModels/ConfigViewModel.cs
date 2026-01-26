namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Presentation.Models;
using System.Collections.ObjectModel;

/// <summary>
/// ViewModel for the column configuration view.
/// Allows users to select which DBF columns to verify during processing.
/// Dynamically loads all 735 columns from VdeConstants.
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
            "Condicion",
            "Proveedor Referidor",
            "Fechas",
            "Hospitalizacion",
            "Laboratorio",
            "Diagnosticos",
            "Resubmision",
            "Impuestos",
            "Montos",
            "Facilidad",
            "Proveedor Facturacion",
            "Linea 1-7",
            "Linea 8-14",
            "Linea 15-21",
            "Linea 22-28"
        ];

        List<string> allColumnNames = VdeConstants.GetAllExpectedColumns();
        ObservableCollection<ColumnConfigItem> columns = [];

        foreach (string columnName in allColumnNames)
        {
            ColumnConfigItem item = CreateColumnConfigItem(columnName);
            columns.Add(item);
        }

        AllColumns = columns;
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

    private static ColumnConfigItem CreateColumnConfigItem(string columnName)
    {
        (string category, string boxReference) = GetCategoryAndBox(columnName);
        string displayName = GetDisplayName(columnName);
        string description = GetDescription(columnName);
        bool isSelected = IsDefaultSelected(columnName);

        return new ColumnConfigItem
        {
            ColumnName = columnName,
            DisplayName = displayName,
            BoxReference = boxReference,
            Category = category,
            Description = description,
            IsSelected = isSelected
        };
    }

    private static (string Category, string BoxReference) GetCategoryAndBox(string columnName)
    {
        // Document metadata (V0*)
        if (columnName.StartsWith("V0"))
            return ("Documento", "N/A");

        // Page number
        if (columnName == "V1PAGINA")
            return ("Documento", "N/A");

        // Patient info Box 1-4 (V11*, V12*, V13*, V14*)
        if (columnName.StartsWith("V11") || columnName.StartsWith("V12") ||
            columnName.StartsWith("V13") || columnName.StartsWith("V14"))
        {
            string box = columnName switch
            {
                var n when n.StartsWith("V11A") => "Box 1a",
                var n when n.StartsWith("V11") => "Box 1",
                var n when n.StartsWith("V12") => "Box 2",
                var n when n.StartsWith("V13") => "Box 3",
                var n when n.StartsWith("V14") => "Box 4",
                _ => "Box 1-4"
            };
            return ("Paciente", box);
        }

        // Patient address Box 5 (V15*)
        if (columnName.StartsWith("V15"))
            return ("Paciente", "Box 5");

        // Patient relationship Box 6 (V16*)
        if (columnName.StartsWith("V16"))
            return ("Paciente", "Box 6");

        // Insured address Box 7 (V17*)
        if (columnName.StartsWith("V17"))
            return ("Asegurado", "Box 7");

        // Reserved Box 8 (V28*)
        if (columnName.StartsWith("V28"))
            return ("Asegurado", "Box 8");

        // Other insured Box 9 (V29*)
        if (columnName.StartsWith("V29"))
            return ("Asegurado", "Box 9");

        // Condition related Box 10 (V210*)
        if (columnName.StartsWith("V210"))
            return ("Condicion", "Box 10");

        // Insured policy Box 11 (V211*)
        if (columnName.StartsWith("V211"))
            return ("Asegurado", "Box 11");

        // Signatures Box 12-13 (V212*, V213*)
        if (columnName.StartsWith("V212") || columnName.StartsWith("V213"))
            return ("Paciente", columnName.StartsWith("V212") ? "Box 12" : "Box 13");

        // Dates Box 14-16 (V314*, V315*, V316*)
        if (columnName.StartsWith("V314") || columnName.StartsWith("V315") || columnName.StartsWith("V316"))
        {
            string box = columnName switch
            {
                var n when n.StartsWith("V314") => "Box 14",
                var n when n.StartsWith("V315") => "Box 15",
                var n when n.StartsWith("V316") => "Box 16",
                _ => "Box 14-16"
            };
            return ("Fechas", box);
        }

        // Referring provider Box 17 (V317*)
        if (columnName.StartsWith("V317"))
            return ("Proveedor Referidor", "Box 17");

        // Hospitalization Box 18-19 (V318*, V319*)
        if (columnName.StartsWith("V318") || columnName.StartsWith("V319"))
            return ("Hospitalizacion", columnName.StartsWith("V318") ? "Box 18" : "Box 19");

        // Outside lab Box 20 (V320*)
        if (columnName.StartsWith("V320"))
            return ("Laboratorio", "Box 20");

        // Diagnosis Box 21 (V321*)
        if (columnName.StartsWith("V321"))
            return ("Diagnosticos", "Box 21");

        // Resubmission Box 22 (V322*, V422*)
        if (columnName.StartsWith("V322") || columnName.StartsWith("V422"))
            return ("Resubmision", "Box 22");

        // Prior auth Box 23 (V423*)
        if (columnName.StartsWith("V423"))
            return ("Resubmision", "Box 23");

        // Tax Box 25 (V425*)
        if (columnName.StartsWith("V425"))
            return ("Impuestos", "Box 25");

        // Patient account Box 26 (V426*)
        if (columnName.StartsWith("V426"))
            return ("Paciente", "Box 26");

        // Accept assignment Box 27 (V427*)
        if (columnName.StartsWith("V427"))
            return ("Montos", "Box 27");

        // Amounts Box 28-30 (V428*, V429*, V430*)
        if (columnName.StartsWith("V428") || columnName.StartsWith("V429") || columnName.StartsWith("V430"))
        {
            string box = columnName switch
            {
                var n when n.StartsWith("V428") => "Box 28",
                var n when n.StartsWith("V429") => "Box 29",
                var n when n.StartsWith("V430") => "Box 30",
                _ => "Box 28-30"
            };
            return ("Montos", box);
        }

        // Signature Box 31 (V431*)
        if (columnName.StartsWith("V431"))
            return ("Proveedor Facturacion", "Box 31");

        // Service facility Box 32 (V432*)
        if (columnName.StartsWith("V432"))
            return ("Facilidad", "Box 32");

        // Billing provider Box 33 (V433*)
        if (columnName.StartsWith("V433"))
            return ("Proveedor Facturacion", "Box 33");

        // Service lines (V5* through VW*)
        int? lineNumber = GetServiceLineNumber(columnName);
        if (lineNumber.HasValue)
        {
            string category = lineNumber.Value switch
            {
                >= 1 and <= 7 => "Linea 1-7",
                >= 8 and <= 14 => "Linea 8-14",
                >= 15 and <= 21 => "Linea 15-21",
                >= 22 and <= 28 => "Linea 22-28",
                _ => "Lineas de Servicio"
            };
            return (category, $"Box 24 L{lineNumber}");
        }

        return ("Documento", "N/A");
    }

    private static int? GetServiceLineNumber(string columnName)
    {
        if (string.IsNullOrEmpty(columnName) || columnName.Length < 2)
            return null;

        string prefix = columnName[..2];

        return prefix switch
        {
            "V5" => 1,
            "V6" => 2,
            "V7" => 3,
            "V8" => 4,
            "V9" => 5,
            "VA" => 6,
            "VB" => 7,
            "VC" => 8,
            "VD" => 9,
            "VE" => 10,
            "VF" => 11,
            "VG" => 12,
            "VH" => 13,
            "VI" => 14,
            "VJ" => 15,
            "VK" => 16,
            "VL" => 17,
            "VM" => 18,
            "VN" => 19,
            "VO" => 20,
            "VP" => 21,
            "VQ" => 22,
            "VR" => 23,
            "VS" => 24,
            "VT" => 25,
            "VU" => 26,
            "VV" => 27,
            "VW" => 28,
            _ => null
        };
    }

    private static string GetDisplayName(string columnName)
    {
        // Service line fields
        int? lineNumber = GetServiceLineNumber(columnName);
        if (lineNumber.HasValue)
        {
            string suffix = columnName.Length > 2 ? columnName[2..] : "";
            string fieldName = suffix switch
            {
                "24ADATEF" => "Fecha Desde",
                "24ADATET" => "Fecha Hasta",
                "24BPLACE" => "Place of Service",
                "24CEMG" => "Emergencia",
                "24ABBMBB" => "ABB/MBB",
                "24DCPT" => "CPT/HCPCS",
                "24DMOD1" => "Modificador 1",
                "24DMOD2" => "Modificador 2",
                "24DMOD3" => "Modificador 3",
                "24DMOD4" => "Modificador 4",
                "24EDIAGN" => "Pointer Dx",
                "24FCHARG" => "Cargos",
                "24GDAYS" => "Dias/Unidades",
                "24HEPSOT" => "EPSDT",
                "24IQUAL" => "ID Qualifier",
                "24JTAXON" => "Taxonomy",
                "24JNPI" => "Rendering NPI",
                "24NDCQUA" => "NDC Qualifier",
                "24NDC" => "NDC Code",
                "24UNITQU" => "Unit Qualifier",
                "24UNIT" => "Unit",
                "CHANGE" => "Change Flag",
                _ => suffix
            };
            return $"L{lineNumber}: {fieldName}";
        }

        // Non-service-line fields
        return columnName switch
        {
            // Document metadata
            "V0DOCUMENT" => "Numero Documento",
            "V0BATCHNUM" => "Numero de Batch",
            "V0SEQUENCE" => "Secuencia",
            "V0CURSTAGE" => "Etapa Actual",
            "V0EXPRUNID" => "Export Run ID",
            "V0KEYOPER" => "Operador Key",
            "V0VFYOPER" => "Operador Verify",
            "V0VIEWNAME" => "Nombre Vista",
            "V0FILEPATH" => "Ruta Archivo",
            "V0IFNAME01" => "Nombre Imagen",
            "V0CONFIDNC" => "Confianza",
            "V1PAGINA" => "Pagina",

            // Patient info
            "V11TYPEID" => "Tipo Cobertura",
            "V11AZUA" => "Numero ZUA",
            "V11AINSURE" => "ID Asegurado",
            "V12LASTNAM" => "Apellido Paciente",
            "V12NAME" => "Nombre Paciente",
            "V12INITIAL" => "Inicial Paciente",
            "V13BIRTH" => "Fecha Nacimiento",
            "V13SEXO" => "Sexo Paciente",

            // Insured Box 4
            "V14LASTNAM" => "Apellido Asegurado",
            "V14NAME" => "Nombre Asegurado",
            "V14INITIAL" => "Inicial Asegurado",

            // Patient address Box 5
            "V15ADDRES1" => "Direccion 1",
            "V15ADDRES2" => "Direccion 2",
            "V15CITY" => "Ciudad",
            "V15STATE" => "Estado",
            "V15ZIPCODE" => "Codigo Postal",
            "V15PLUS4" => "ZIP+4",
            "V15TELEFON" => "Telefono",

            // Patient relationship Box 6
            "V16PATRELA" => "Relacion Paciente",

            // Insured address Box 7
            "V17ADDRES1" => "Dir. Asegurado 1",
            "V17ADDRES2" => "Dir. Asegurado 2",
            "V17CITY" => "Ciudad Asegurado",
            "V17STATE" => "Estado Asegurado",
            "V17ZIPCODE" => "ZIP Asegurado",
            "V17PLUS4" => "ZIP+4 Asegurado",
            "V17TELEFON" => "Tel. Asegurado",

            // Box 8-9
            "V28RESERVE" => "Reservado",
            "V29LASTNAM" => "Apellido Otro Aseg.",
            "V29NAME" => "Nombre Otro Aseg.",
            "V29INITIAL" => "Inicial Otro Aseg.",
            "V29APOLICY" => "Poliza Otro Aseg.",
            "V29BRESERV" => "Reservado 9b",
            "V29CRESERV" => "Reservado 9c",
            "V29DINSPLA" => "Plan de Seguro",

            // Box 10
            "V210AEMPLO" => "Rel. Empleo",
            "V210BAUTO" => "Accidente Auto",
            "V210COTHER" => "Otro Accidente",
            "V210STATE" => "Estado Accidente",
            "V210DCLAIM" => "Claim Codes",

            // Box 11
            "V211INSURE" => "Poliza Asegurado",
            "V211ABIRTH" => "Fecha Nac. Aseg.",
            "V211ASEXO" => "Sexo Asegurado",
            "V211BQUAL" => "Qualifier 11b",
            "V211BOTHER" => "Otro ID 11b",
            "V211CINSUR" => "Plan Seguro",
            "V211DISTHE" => "Otro Seguro?",

            // Box 12-13
            "V212SIGNED" => "Firma Paciente",
            "V212DATE" => "Fecha Firma Pac.",
            "V213FIRMA" => "Autoriza Pago",

            // Box 14-16
            "V314DATE" => "Fecha Condicion",
            "V314QUAL" => "Qualifier 14",
            "V315QUAL" => "Qualifier 15",
            "V315DATE" => "Fecha Similar",
            "V316DATEFR" => "Incapacidad Desde",
            "V316DATETO" => "Incapacidad Hasta",

            // Box 17
            "V317QUAL" => "Qualifier 17",
            "V317NAME" => "Nombre Referidor",
            "V317AQUAL" => "Qualifier 17a",
            "V317AREFFE" => "ID Referidor",
            "V317BNPI" => "NPI Referidor",

            // Box 18-19
            "V318DATEFR" => "Admision Desde",
            "V318DATETO" => "Admision Hasta",
            "V319ADD" => "Info Adicional",

            // Box 20
            "V320OUTSID" => "Lab Externo?",
            "V320CHARGE" => "Cargo Lab",

            // Box 21 - Diagnosis
            "V321ICDIND" => "Indicador ICD",
            "V321DIAG1" => "Diagnostico A",
            "V321DIAG2" => "Diagnostico B",
            "V321DIAG3" => "Diagnostico C",
            "V321DIAG4" => "Diagnostico D",
            "V321DIAG5" => "Diagnostico E",
            "V321DIAG6" => "Diagnostico F",
            "V321DIAG7" => "Diagnostico G",
            "V321DIAG8" => "Diagnostico H",
            "V321DIAG9" => "Diagnostico I",
            "V321DIAG10" => "Diagnostico J",
            "V321DIAG11" => "Diagnostico K",
            "V321DIAG12" => "Diagnostico L",

            // Box 22-23
            "V322RESUB" => "Cod. Resubmision",
            "V422ORIGIN" => "Claim Original",
            "V423PRIOR" => "Auth. Previa",

            // Box 25-26
            "V425FEDTAX" => "Fed Tax ID",
            "V425SSN" => "SSN",
            "V425EIN" => "EIN",
            "V426PATIEN" => "Cuenta Paciente",

            // Box 27-30
            "V427ACCEPT" => "Acepta Asignacion",
            "V428TOTAL" => "Total Cargos",
            "V429AMOUNT" => "Monto Pagado",
            "V430NUCC" => "Balance NUCC",

            // Box 31
            "V431DATE" => "Fecha Firma Prov.",

            // Box 32
            "V432FACILI" => "Facilidad",
            "V432ZIPCOD" => "ZIP Facilidad",
            "V432PLUS4" => "ZIP+4 Facilidad",
            "V432ANPI" => "NPI Facilidad",
            "V432BOTHER" => "Otro ID Facilidad",

            // Box 33
            "V433ANPI" => "NPI Facturacion",
            "V433BQUAL" => "Qualifier 33b",
            "V433BTAXON" => "Taxonomy",
            "V433NAME" => "Nombre Prov.",
            "V433INITIA" => "Inicial Prov.",
            "V433LASTNA" => "Apellido Prov.",
            "V433ADDRE1" => "Direccion Prov.",
            "V433ADDRE2" => "Direccion 2 Prov.",
            "V433CITY" => "Ciudad Prov.",
            "V433STATE" => "Estado Prov.",
            "V433ZIPCOD" => "ZIP Prov.",
            "V433PLUS4" => "ZIP+4 Prov.",

            _ => columnName
        };
    }

    private static string GetDescription(string columnName)
    {
        // Service line fields
        int? lineNumber = GetServiceLineNumber(columnName);
        if (lineNumber.HasValue)
        {
            string suffix = columnName.Length > 2 ? columnName[2..] : "";
            string desc = suffix switch
            {
                "24ADATEF" => "Fecha de servicio desde",
                "24ADATET" => "Fecha de servicio hasta",
                "24BPLACE" => "Codigo lugar de servicio",
                "24CEMG" => "Indicador de emergencia",
                "24ABBMBB" => "Indicador ABB/MBB",
                "24DCPT" => "Codigo de procedimiento",
                "24DMOD1" => "Primer modificador",
                "24DMOD2" => "Segundo modificador",
                "24DMOD3" => "Tercer modificador",
                "24DMOD4" => "Cuarto modificador",
                "24EDIAGN" => "Apuntador a diagnosticos",
                "24FCHARG" => "Monto del cargo",
                "24GDAYS" => "Dias o unidades de servicio",
                "24HEPSOT" => "EPSDT/Family Plan",
                "24IQUAL" => "Calificador de ID",
                "24JTAXON" => "Codigo taxonomy del rendering",
                "24JNPI" => "NPI del rendering provider",
                "24NDCQUA" => "Calificador NDC",
                "24NDC" => "National Drug Code",
                "24UNITQU" => "Calificador de unidad",
                "24UNIT" => "Cantidad de unidad",
                "CHANGE" => "Flag de cambio en linea",
                _ => $"Campo de linea de servicio {lineNumber}"
            };
            return $"{desc} (Linea {lineNumber})";
        }

        // Default description based on column name
        return columnName switch
        {
            "V0DOCUMENT" => "Identificador unico del documento",
            "V0BATCHNUM" => "Numero del lote de procesamiento",
            "V0SEQUENCE" => "Numero de secuencia en el batch",
            "V0IFNAME01" => "Ruta del archivo de imagen",
            "V11AINSURE" => "Numero de identificacion del asegurado",
            "V13BIRTH" => "Fecha de nacimiento del paciente",
            "V317BNPI" => "NPI del proveedor que refiere",
            "V321ICDIND" => "9=ICD-9, 0=ICD-10",
            "V428TOTAL" => "Total de cargos de la reclamacion",
            "V432ANPI" => "NPI de la facilidad de servicio",
            "V433ANPI" => "NPI del proveedor de facturacion",
            _ => $"Campo {columnName} del formulario CMS-1500"
        };
    }

    private static bool IsDefaultSelected(string columnName)
    {
        // Select important fields by default
        HashSet<string> defaultSelected =
        [
            "V11AINSURE", "V13BIRTH", "V317BNPI", "V321ICDIND",
            "V321DIAG1", "V321DIAG2", "V321DIAG3", "V321DIAG4",
            "V428TOTAL", "V432ANPI", "V433ANPI"
        ];

        if (defaultSelected.Contains(columnName))
            return true;

        // Select key service line fields for line 1
        int? lineNumber = GetServiceLineNumber(columnName);
        if (lineNumber == 1)
        {
            string suffix = columnName.Length > 2 ? columnName[2..] : "";
            return suffix is "24ADATEF" or "24ADATET" or "24BPLACE" or
                   "24DCPT" or "24EDIAGN" or "24FCHARG" or "24GDAYS" or "24JNPI";
        }

        return false;
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
