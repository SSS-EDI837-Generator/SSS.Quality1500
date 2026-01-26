namespace SSS.Quality1500.Domain.Constants;

/// <summary>
/// Constants for VDE (Virtual Data Entry) record fields.
/// Based on actual DBF structure from SSS1503.DBF.
/// For service line constants (Box 24), see VdeServiceLineConstants.
/// </summary>
public static class VdeConstants
{
    #region Document Metadata (V0*)

    public const string V0Document = "V0DOCUMENT";
    public const string V0Batchnum = "V0BATCHNUM";
    public const string V0Sequence = "V0SEQUENCE";
    public const string V0CurStage = "V0CURSTAGE";
    public const string V0ExpRunId = "V0EXPRUNID";
    public const string V0KeyOper = "V0KEYOPER";
    public const string V0VfyOper = "V0VFYOPER";
    public const string V0ViewName = "V0VIEWNAME";
    public const string V0FilePath = "V0FILEPATH";
    public const string V0IfName01 = "V0IFNAME01";
    public const string V0Confidnc = "V0CONFIDNC";

    #endregion

    #region Page Number (V1*)

    public const string V1Page = "V1PAGINA";

    #endregion

    #region Patient Information - Box 1-4 (V1*)

    /// <summary>Box 1: Insurance type</summary>
    public const string V11TypeId = "V11TYPEID";

    /// <summary>Box 1a: ZUA number</summary>
    public const string V11AZua = "V11AZUA";

    /// <summary>Box 1a: Insured's ID number</summary>
    public const string V11AInsure = "V11AINSURE";

    /// <summary>Box 2: Patient's last name</summary>
    public const string V12LastName = "V12LASTNAM";

    /// <summary>Box 2: Patient's first name</summary>
    public const string V12Name = "V12NAME";

    /// <summary>Box 2: Patient's middle initial</summary>
    public const string V12Initial = "V12INITIAL";

    /// <summary>Box 3: Patient's birth date</summary>
    public const string V13Birth = "V13BIRTH";

    /// <summary>Box 3: Patient's sex</summary>
    public const string V13Sexo = "V13SEXO";

    #endregion

    #region Insured Information - Box 4 (V1*)

    /// <summary>Box 4: Insured's last name</summary>
    public const string V14LastName = "V14LASTNAM";

    /// <summary>Box 4: Insured's first name</summary>
    public const string V14Name = "V14NAME";

    /// <summary>Box 4: Insured's middle initial</summary>
    public const string V14Initial = "V14INITIAL";

    #endregion

    #region Patient Address - Box 5 (V1*)

    public const string V15Address1 = "V15ADDRES1";
    public const string V15Address2 = "V15ADDRES2";
    public const string V15City = "V15CITY";
    public const string V15State = "V15STATE";
    public const string V15ZipCode = "V15ZIPCODE";
    public const string V15Plus4 = "V15PLUS4";
    public const string V15Telefon = "V15TELEFON";

    #endregion

    #region Patient Relationship - Box 6 (V1*)

    public const string V16PatRela = "V16PATRELA";

    #endregion

    #region Insured Address - Box 7 (V1*)

    public const string V17Address1 = "V17ADDRES1";
    public const string V17Address2 = "V17ADDRES2";
    public const string V17City = "V17CITY";
    public const string V17State = "V17STATE";
    public const string V17ZipCode = "V17ZIPCODE";
    public const string V17Plus4 = "V17PLUS4";
    public const string V17Telefon = "V17TELEFON";

    #endregion

    #region Reserved and Other Insured - Box 8-9 (V2*)

    public const string V28Reserve = "V28RESERVE";
    public const string V29LastName = "V29LASTNAM";
    public const string V29Name = "V29NAME";
    public const string V29Initial = "V29INITIAL";
    public const string V29APolicy = "V29APOLICY";
    public const string V29BReserv = "V29BRESERV";
    public const string V29CReserv = "V29CRESERV";
    public const string V29DInsPla = "V29DINSPLA";

    #endregion

    #region Condition Related To - Box 10 (V2*)

    public const string V210AEmplo = "V210AEMPLO";
    public const string V210BAuto = "V210BAUTO";
    public const string V210COther = "V210COTHER";
    public const string V210State = "V210STATE";
    public const string V210DClaim = "V210DCLAIM";

    #endregion

    #region Insured Policy - Box 11 (V2*)

    public const string V211Insure = "V211INSURE";
    public const string V211ABirth = "V211ABIRTH";
    public const string V211ASexo = "V211ASEXO";
    public const string V211BQual = "V211BQUAL";
    public const string V211BOther = "V211BOTHER";
    public const string V211CInsur = "V211CINSUR";
    public const string V211DIsThere = "V211DISTHE";

    #endregion

    #region Signatures - Box 12-13 (V2*)

    public const string V212Signed = "V212SIGNED";
    public const string V212Date = "V212DATE";
    public const string V213Firma = "V213FIRMA";

    #endregion

    #region Dates - Box 14-16 (V3*)

    public const string V314Date = "V314DATE";
    public const string V314Qual = "V314QUAL";
    public const string V315Qual = "V315QUAL";
    public const string V315Date = "V315DATE";
    public const string V316DateFr = "V316DATEFR";
    public const string V316DateTo = "V316DATETO";

    #endregion

    #region Referring Provider - Box 17 (V3*)

    public const string V317Qual = "V317QUAL";
    public const string V317Name = "V317NAME";
    public const string V317AQual = "V317AQUAL";
    public const string V317AReffE = "V317AREFFE";
    public const string V317BNpi = "V317BNPI";

    #endregion

    #region Hospitalization - Box 18-19 (V3*)

    public const string V318DateFr = "V318DATEFR";
    public const string V318DateTo = "V318DATETO";
    public const string V319Add = "V319ADD";

    #endregion

    #region Outside Lab - Box 20 (V3*)

    public const string V320OutSid = "V320OUTSID";
    public const string V320Charge = "V320CHARGE";

    #endregion

    #region Diagnosis - Box 21 (V3*)

    public const string V321IcdInd = "V321ICDIND";
    public const string V321Diag1 = "V321DIAG1";
    public const string V321Diag2 = "V321DIAG2";
    public const string V321Diag3 = "V321DIAG3";
    public const string V321Diag4 = "V321DIAG4";
    public const string V321Diag5 = "V321DIAG5";
    public const string V321Diag6 = "V321DIAG6";
    public const string V321Diag7 = "V321DIAG7";
    public const string V321Diag8 = "V321DIAG8";
    public const string V321Diag9 = "V321DIAG9";
    public const string V321Diag10 = "V321DIAG10";
    public const string V321Diag11 = "V321DIAG11";
    public const string V321Diag12 = "V321DIAG12";

    #endregion

    #region Resubmission and Prior Authorization - Box 22-23 (V3*, V4*)

    public const string V322Resub = "V322RESUB";
    public const string V422Origin = "V422ORIGIN";
    public const string V423Prior = "V423PRIOR";

    #endregion

    #region Tax and Patient Account - Box 25-26 (V4*)

    public const string V425FedTax = "V425FEDTAX";
    public const string V425Ssn = "V425SSN";
    public const string V425Ein = "V425EIN";
    public const string V426Patien = "V426PATIEN";

    #endregion

    #region Accept Assignment and Amounts - Box 27-30 (V4*)

    public const string V427Accept = "V427ACCEPT";
    public const string V428Total = "V428TOTAL";
    public const string V429Amount = "V429AMOUNT";
    public const string V430Nucc = "V430NUCC";

    #endregion

    #region Signature of Physician - Box 31 (V4*)

    public const string V431Date = "V431DATE";

    #endregion

    #region Service Facility - Box 32 (V4*)

    public const string V432Facili = "V432FACILI";
    public const string V432ZipCod = "V432ZIPCOD";
    public const string V432Plus4 = "V432PLUS4";
    public const string V432ANpi = "V432ANPI";
    public const string V432BOther = "V432BOTHER";

    #endregion

    #region Billing Provider - Box 33 (V4*)

    public const string V433ANpi = "V433ANPI";
    public const string V433BQual = "V433BQUAL";
    public const string V433BTaxon = "V433BTAXON";
    public const string V433Name = "V433NAME";
    public const string V433Initia = "V433INITIA";
    public const string V433LastNa = "V433LASTNA";
    public const string V433Addre1 = "V433ADDRE1";
    public const string V433Addre2 = "V433ADDRE2";
    public const string V433City = "V433CITY";
    public const string V433State = "V433STATE";
    public const string V433ZipCod = "V433ZIPCOD";
    public const string V433Plus4 = "V433PLUS4";

    #endregion

    #region Column Validation

    /// <summary>
    /// Gets all non-service-line column names (119 columns).
    /// </summary>
    public static string[] GetNonServiceLineColumns() =>
    [
        // Document Metadata (11)
        V0Document, V0Batchnum, V0Sequence, V0CurStage, V0ExpRunId,
        V0KeyOper, V0VfyOper, V0ViewName, V0FilePath, V0IfName01, V0Confidnc,
        // Page (1)
        V1Page,
        // Patient Info Box 1-4 (8)
        V11TypeId, V11AZua, V11AInsure, V12LastName, V12Name, V12Initial, V13Birth, V13Sexo,
        // Insured Info Box 4 (3)
        V14LastName, V14Name, V14Initial,
        // Patient Address Box 5 (7)
        V15Address1, V15Address2, V15City, V15State, V15ZipCode, V15Plus4, V15Telefon,
        // Patient Relationship Box 6 (1)
        V16PatRela,
        // Insured Address Box 7 (7)
        V17Address1, V17Address2, V17City, V17State, V17ZipCode, V17Plus4, V17Telefon,
        // Box 8-9 (8)
        V28Reserve, V29LastName, V29Name, V29Initial, V29APolicy, V29BReserv, V29CReserv, V29DInsPla,
        // Box 10 (5)
        V210AEmplo, V210BAuto, V210COther, V210State, V210DClaim,
        // Box 11 (7)
        V211Insure, V211ABirth, V211ASexo, V211BQual, V211BOther, V211CInsur, V211DIsThere,
        // Box 12-13 (3)
        V212Signed, V212Date, V213Firma,
        // Box 14-16 (6)
        V314Date, V314Qual, V315Qual, V315Date, V316DateFr, V316DateTo,
        // Box 17 (5)
        V317Qual, V317Name, V317AQual, V317AReffE, V317BNpi,
        // Box 18-19 (3)
        V318DateFr, V318DateTo, V319Add,
        // Box 20 (2)
        V320OutSid, V320Charge,
        // Box 21 - Diagnosis (13)
        V321IcdInd, V321Diag1, V321Diag2, V321Diag3, V321Diag4, V321Diag5, V321Diag6,
        V321Diag7, V321Diag8, V321Diag9, V321Diag10, V321Diag11, V321Diag12,
        // Box 22-23 (3)
        V322Resub, V422Origin, V423Prior,
        // Box 25-26 (4)
        V425FedTax, V425Ssn, V425Ein, V426Patien,
        // Box 27-30 (4)
        V427Accept, V428Total, V429Amount, V430Nucc,
        // Box 31 (1)
        V431Date,
        // Box 32 (5)
        V432Facili, V432ZipCod, V432Plus4, V432ANpi, V432BOther,
        // Box 33 (12)
        V433ANpi, V433BQual, V433BTaxon, V433Name, V433Initia, V433LastNa,
        V433Addre1, V433Addre2, V433City, V433State, V433ZipCod, V433Plus4
    ];

    /// <summary>
    /// Gets all expected column names for DBF validation.
    /// Includes 119 non-service-line columns plus 28 service lines (22 columns each = 616).
    /// Total: 735 columns.
    /// </summary>
    public static List<string> GetAllExpectedColumns()
    {
        List<string> columns = [.. GetNonServiceLineColumns()];

        string[] suffixes = VdeServiceLineConstants.Suffix.GetAllSuffixes();

        for (int line = 1; line <= 28; line++)
        {
            foreach (string suffix in suffixes)
            {
                columns.Add(VdeServiceLineConstants.GetColumnName(line, suffix));
            }
        }

        return columns;
    }

    #endregion

    #region Processing Constants

    public const string Job = "TSA837P";
    public const string Hd = "NS";
    public const string LimiteDateIcd10Icd9 = "093015";
    public const int ViewStart = 5;
    public const int ViewEnd = 32;
    public const bool Icd10OrIcd9 = true;

    #endregion
}
