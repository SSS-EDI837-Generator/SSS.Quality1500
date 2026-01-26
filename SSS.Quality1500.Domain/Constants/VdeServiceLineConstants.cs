namespace SSS.Quality1500.Domain.Constants;

/// <summary>
/// Constants for CMS-1500 Box 24 service lines (lines 1-28).
/// Service lines use prefixes V5-VW for lines 1-28.
/// Based on actual DBF structure from SSS1503.DBF.
/// </summary>
public static class VdeServiceLineConstants
{
    /// <summary>
    /// Service line prefixes for lines 1-28.
    /// Line 1 = V5, Line 2 = V6, ... Line 28 = VW
    /// </summary>
    public static class Prefix
    {
        public const string Line01 = "V5";
        public const string Line02 = "V6";
        public const string Line03 = "V7";
        public const string Line04 = "V8";
        public const string Line05 = "V9";
        public const string Line06 = "VA";
        public const string Line07 = "VB";
        public const string Line08 = "VC";
        public const string Line09 = "VD";
        public const string Line10 = "VE";
        public const string Line11 = "VF";
        public const string Line12 = "VG";
        public const string Line13 = "VH";
        public const string Line14 = "VI";
        public const string Line15 = "VJ";
        public const string Line16 = "VK";
        public const string Line17 = "VL";
        public const string Line18 = "VM";
        public const string Line19 = "VN";
        public const string Line20 = "VO";
        public const string Line21 = "VP";
        public const string Line22 = "VQ";
        public const string Line23 = "VR";
        public const string Line24 = "VS";
        public const string Line25 = "VT";
        public const string Line26 = "VU";
        public const string Line27 = "VV";
        public const string Line28 = "VW";

        /// <summary>
        /// Gets the prefix for a specific line number (1-28).
        /// </summary>
        public static string GetPrefix(int lineNumber)
        {
            return lineNumber switch
            {
                1 => Line01,
                2 => Line02,
                3 => Line03,
                4 => Line04,
                5 => Line05,
                6 => Line06,
                7 => Line07,
                8 => Line08,
                9 => Line09,
                10 => Line10,
                11 => Line11,
                12 => Line12,
                13 => Line13,
                14 => Line14,
                15 => Line15,
                16 => Line16,
                17 => Line17,
                18 => Line18,
                19 => Line19,
                20 => Line20,
                21 => Line21,
                22 => Line22,
                23 => Line23,
                24 => Line24,
                25 => Line25,
                26 => Line26,
                27 => Line27,
                28 => Line28,
                _ => throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number must be 1-28")
            };
        }
    }

    /// <summary>
    /// Service line field suffixes (append to line prefix).
    /// Example: V524ADATEF = Line 1 Date From, V624ADATEF = Line 2 Date From
    /// </summary>
    public static class Suffix
    {
        /// <summary>Box 24A: Date of service - from</summary>
        public const string DateFrom = "24ADATEF";

        /// <summary>Box 24A: Date of service - to</summary>
        public const string DateTo = "24ADATET";

        /// <summary>Box 24B: Place of service</summary>
        public const string PlaceOfService = "24BPLACE";

        /// <summary>Box 24C: EMG (emergency indicator)</summary>
        public const string Emergency = "24CEMG";

        /// <summary>ABB/MBB indicator</summary>
        public const string AbbMbb = "24ABBMBB";

        /// <summary>Box 24D: CPT/HCPCS procedure code</summary>
        public const string Cpt = "24DCPT";

        /// <summary>Box 24D: Modifier 1</summary>
        public const string Mod1 = "24DMOD1";

        /// <summary>Box 24D: Modifier 2</summary>
        public const string Mod2 = "24DMOD2";

        /// <summary>Box 24D: Modifier 3</summary>
        public const string Mod3 = "24DMOD3";

        /// <summary>Box 24D: Modifier 4</summary>
        public const string Mod4 = "24DMOD4";

        /// <summary>Box 24E: Diagnosis pointer</summary>
        public const string DiagPointer = "24EDIAGN";

        /// <summary>Box 24F: Charges</summary>
        public const string Charges = "24FCHARG";

        /// <summary>Box 24G: Days or units</summary>
        public const string DaysUnits = "24GDAYS";

        /// <summary>Box 24H: EPSDT/Family plan</summary>
        public const string Epsdt = "24HEPSOT";

        /// <summary>Box 24I: ID qualifier</summary>
        public const string IdQualifier = "24IQUAL";

        /// <summary>Box 24J: Rendering provider taxonomy</summary>
        public const string RenderingTaxonomy = "24JTAXON";

        /// <summary>Box 24J: Rendering provider NPI</summary>
        public const string RenderingNpi = "24JNPI";

        /// <summary>NDC qualifier</summary>
        public const string NdcQualifier = "24NDCQUA";

        /// <summary>NDC code</summary>
        public const string Ndc = "24NDC";

        /// <summary>Unit qualifier</summary>
        public const string UnitQualifier = "24UNITQU";

        /// <summary>Unit</summary>
        public const string Unit = "24UNIT";

        /// <summary>Change flag for the service line</summary>
        public const string Change = "CHANGE";

        /// <summary>
        /// Gets all service line suffixes as an array (22 suffixes per line).
        /// </summary>
        public static string[] GetAllSuffixes() =>
        [
            DateFrom, DateTo, PlaceOfService, Emergency, AbbMbb,
            Cpt, Mod1, Mod2, Mod3, Mod4, DiagPointer, Charges, DaysUnits,
            Epsdt, IdQualifier, RenderingTaxonomy, RenderingNpi,
            NdcQualifier, Ndc, UnitQualifier, Unit, Change
        ];
    }

    /// <summary>
    /// Total number of service lines in the DBF.
    /// </summary>
    public const int TotalServiceLines = 28;

    /// <summary>
    /// Number of columns per service line.
    /// </summary>
    public const int ColumnsPerLine = 22;

    /// <summary>
    /// Gets the full column name for a service line field.
    /// </summary>
    /// <param name="lineNumber">Line number (1-28)</param>
    /// <param name="suffix">Field suffix from Suffix class</param>
    /// <returns>Full column name (e.g., "V524ADATEF" for line 1 date from)</returns>
    public static string GetColumnName(int lineNumber, string suffix)
    {
        if (lineNumber < 1 || lineNumber > 28)
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number must be 1-28");

        // Special case: CHANGE suffix doesn't have "24" prefix
        if (suffix == Suffix.Change)
            return Prefix.GetPrefix(lineNumber) + suffix;

        return Prefix.GetPrefix(lineNumber) + suffix;
    }
}
