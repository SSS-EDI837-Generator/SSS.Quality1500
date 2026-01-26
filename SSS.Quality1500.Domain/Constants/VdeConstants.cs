namespace SSS.Quality1500.Domain.Constants;

/// <summary>
/// Constants for VDE (Virtual Data Entry) record fields.
/// Organized by CMS-1500 form sections and DBF column prefixes.
/// For service line constants (Box 24), see VdeServiceLineConstants.
/// </summary>
public static class VdeConstants
{
    #region Document Metadata (V0*)

    /// <summary>Unique document identifier</summary>
    public const string V0Document = "V0DOCUMENT";

    /// <summary>Batch number for processing</summary>
    public const string V0Batchnum = "V0BATCHNUM";

    /// <summary>Sequence number within batch</summary>
    public const string V0Sequence = "V0SEQUENCE";

    /// <summary>Image file path</summary>
    public const string V0IfName01 = "V0IFNAME01";

    /// <summary>Page number</summary>
    public const string V1Page = "V1PAGINA";

    #endregion

    #region Patient Information - Box 1-4 (V1*)

    /// <summary>Box 1: Insurance type (Medicare/Medicaid/etc)</summary>
    public const string V11TypeId = "V11TYPEID";

    /// <summary>Box 1a: Insured's ID number</summary>
    public const string V11AInsure = "V11AINSURE";

    /// <summary>Box 1a: ZUA number</summary>
    public const string V11AZua = "V11AZUA";

    /// <summary>Box 2: Patient's first name</summary>
    public const string V12Name = "V12NAME";

    /// <summary>Box 2: Patient's last name</summary>
    public const string V12LastName = "V12LASTNAM";

    /// <summary>Box 2: Patient's middle initial</summary>
    public const string V12Initial = "V12INITIAL";

    /// <summary>Box 3: Patient's birth date</summary>
    public const string V13Birth = "V13BIRTH";

    /// <summary>Box 3: Patient's sex (M/F)</summary>
    public const string V13Sexo = "V13SEXO";

    #endregion

    #region Patient Address - Box 5 (V1*)

    /// <summary>Box 5: Address line 1</summary>
    public const string V15Address1 = "V15ADDRES1";

    /// <summary>Box 5: Address line 2</summary>
    public const string V15Address2 = "V15ADDRES2";

    /// <summary>Box 5: City</summary>
    public const string V15City = "V15CITY";

    /// <summary>Box 5: State</summary>
    public const string V15State = "V15STATE";

    /// <summary>Box 5: ZIP code</summary>
    public const string V15ZipCode = "V15ZIPCODE";

    /// <summary>Box 5: ZIP+4 extension</summary>
    public const string V15Plus4 = "V15PLUS4";

    /// <summary>Box 5: Telephone number</summary>
    public const string V15Telefon = "V15TELEFON";

    #endregion

    #region Patient Relationship - Box 6 (V1*)

    /// <summary>Box 6: Patient relationship to insured</summary>
    public const string V16PatRela = "V16PATRELA";

    #endregion

    #region Insured Information - Box 7, 9, 11 (V1*, V2*)

    /// <summary>Box 7: Insured's telephone</summary>
    public const string V17Telefon = "V17TELEFON";

    /// <summary>Box 8: Reserved for NUCC use</summary>
    public const string V28Reserve = "V28RESERVE";

    /// <summary>Box 9: Other insured's name</summary>
    public const string V29Name = "V29NAME";

    /// <summary>Box 9: Other insured's initial</summary>
    public const string V29Initial = "V29INITIAL";

    /// <summary>Box 9: Other insured's last name</summary>
    public const string V29LastName = "V29LASTNAM";

    /// <summary>Box 9a: Other insured's policy/group number</summary>
    public const string V29APolicy = "V29APOLICY";

    /// <summary>Box 9b: Reserved for NUCC use</summary>
    public const string V29BReserv = "V29BRESERV";

    /// <summary>Box 9d: Insurance plan name</summary>
    public const string V29DInsPla = "V29DINSPLA";

    #endregion

    #region Condition Related To - Box 10 (V2*)

    /// <summary>Box 10a: Is condition related to employment</summary>
    public const string V210AEmplo = "V210AEMPLO";

    /// <summary>Box 10b: Auto accident</summary>
    public const string V210BAuto = "V210BAUTO";

    /// <summary>Box 10b: State</summary>
    public const string V210State = "V210STATE";

    /// <summary>Box 10c: Other accident</summary>
    public const string V210COther = "V210COTHER";

    /// <summary>Box 10d: Claim codes (designated by NUCC)</summary>
    public const string V210DClaim = "V210DCLAIM";

    #endregion

    #region Insured Policy - Box 11 (V2*)

    /// <summary>Box 11a: Insured's birth date</summary>
    public const string V211ABirth = "V211ABIRTH";

    /// <summary>Box 11a: Insured's sex</summary>
    public const string V211ASexo = "V211ASEXO";

    /// <summary>Box 11b: Other claim ID (designated by NUCC)</summary>
    public const string V211BOther = "V211BOTHER";

    /// <summary>Box 11b: Qualifier</summary>
    public const string V211BQual = "V211BQUAL";

    /// <summary>Box 11c: Insurance plan name or program name</summary>
    public const string V211Insure = "V211INSURE";

    /// <summary>Box 11c: Insured insurance</summary>
    public const string V211CInsur = "V211CINSUR";

    /// <summary>Box 11d: Is there another health benefit plan</summary>
    public const string V211DIsThere = "V211DISTHE";

    #endregion

    #region Signatures - Box 12-13 (V2*)

    /// <summary>Box 12: Patient or authorized person's signature</summary>
    public const string V212Signed = "V212SIGNED";

    /// <summary>Box 13: Insured's or authorized person's signature</summary>
    public const string V213Firma = "V213FIRMA";

    #endregion

    #region Dates and Referring Provider - Box 14-19 (V3*)

    /// <summary>Box 14: Date of current illness/injury/pregnancy (qualifier)</summary>
    public const string V314Qual = "V314QUAL";

    /// <summary>Box 14: Date of current illness/injury/pregnancy</summary>
    public const string V314Date = "V314DATE";

    /// <summary>Box 15: Other date (qualifier)</summary>
    public const string V315Qual = "V315QUAL";

    /// <summary>Box 15: Other date</summary>
    public const string V315Date = "V315DATE";

    /// <summary>Box 16: Dates patient unable to work - from</summary>
    public const string V316DateFr = "V316DATEFR";

    /// <summary>Box 16: Dates patient unable to work - to</summary>
    public const string V316DateTo = "V316DATETO";

    /// <summary>Box 17: Referring provider name</summary>
    public const string V317Name = "V317NAME";

    /// <summary>Box 17a: Qualifier</summary>
    public const string V317AQual = "V317AQUAL";

    /// <summary>Box 17a: Other ID number</summary>
    public const string V317AReffE = "V317AREFFE";

    /// <summary>Box 17b: Referring provider NPI</summary>
    public const string V317BNpi = "V317BNPI";

    /// <summary>Box 18: Hospitalization dates - from</summary>
    public const string V318DateFr = "V318DATEFR";

    /// <summary>Box 18: Hospitalization dates - to</summary>
    public const string V318DateTo = "V318DATETO";

    /// <summary>Box 19: Additional claim information</summary>
    public const string V319Add = "V319ADD";

    #endregion

    #region Outside Lab and Diagnosis - Box 20-21 (V3*)

    /// <summary>Box 20: Outside lab</summary>
    public const string V320OutSid = "V320OUTSID";

    /// <summary>Box 20: Charges</summary>
    public const string V320Charge = "V320CHARGE";

    /// <summary>Box 21: ICD indicator (9 = ICD-9, 0 = ICD-10)</summary>
    public const string V321IcdInd = "V321ICDIND";

    /// <summary>Box 21: Diagnosis or nature of illness codes (A-L)</summary>
    public const string V321Diag = "V321DIAG";

    #endregion

    #region Resubmission and Prior Authorization - Box 22-23 (V3*, V4*)

    /// <summary>Box 22: Resubmission code</summary>
    public const string V322Resub = "V322RESUB";

    /// <summary>Box 23: Prior authorization number</summary>
    public const string V423Prior = "V423PRIOR";

    #endregion

    #region Tax and Patient Account - Box 25-26 (V4*)

    /// <summary>Box 25: Federal tax ID number</summary>
    public const string V425FedTax = "V425FEDTAX";

    /// <summary>Box 25: SSN indicator</summary>
    public const string V425Ssn = "V425SSN";

    /// <summary>Box 25: EIN indicator</summary>
    public const string V425Ein = "V425EIN";

    /// <summary>Box 26: Patient's account number</summary>
    public const string V426Patien = "V426PATIEN";

    #endregion

    #region Accept Assignment and Amounts - Box 27-30 (V4*)

    /// <summary>Box 27: Accept assignment</summary>
    public const string V427Accept = "V427ACCEPT";

    /// <summary>Box 28: Total charge</summary>
    public const string V428Total = "V428TOTAL";

    /// <summary>Box 29: Amount paid</summary>
    public const string V429Amount = "V429AMOUNT";

    /// <summary>Box 30: NUCC reserved</summary>
    public const string V430Nucc = "V430NUCC";

    #endregion

    #region Signature of Physician - Box 31 (V4*)

    /// <summary>Box 31: Signature of physician date</summary>
    public const string V431Date = "V431DATE";

    #endregion

    #region Service Facility - Box 32 (V4*)

    /// <summary>Box 32: Service facility location - name</summary>
    public const string V432Name = "V432NAME";

    /// <summary>Box 32: Service facility location - address 1</summary>
    public const string V432Addre1 = "V432ADDRE1";

    /// <summary>Box 32: Service facility location - address 2</summary>
    public const string V432Addre2 = "V432ADDRE2";

    /// <summary>Box 32: Service facility location - city</summary>
    public const string V432City = "V432CITY";

    /// <summary>Box 32: Service facility location - state</summary>
    public const string V432State = "V432STATE";

    /// <summary>Box 32: Service facility location - ZIP code</summary>
    public const string V432ZipCod = "V432ZIPCOD";

    /// <summary>Box 32: Service facility location - ZIP+4</summary>
    public const string V432Plus4 = "V432PLUS4";

    /// <summary>Box 32a: Service facility NPI</summary>
    public const string V432ANpi = "V432ANPI";

    /// <summary>Box 32b: Service facility other ID</summary>
    public const string V432BOther = "V432BOTHER";

    /// <summary>Box 32b: Service facility other ID qualifier</summary>
    public const string V432BQual = "V432BQUAL";

    #endregion

    #region Billing Provider - Box 33 (V4*)

    /// <summary>Box 33: Billing provider name</summary>
    public const string V433Name = "V433NAME";

    /// <summary>Box 33: Billing provider last name</summary>
    public const string V433LastNa = "V433LASTNA";

    /// <summary>Box 33: Billing provider initial</summary>
    public const string V433Initia = "V433INITIA";

    /// <summary>Box 33: Billing provider address 1</summary>
    public const string V433Addre1 = "V433ADDRE1";

    /// <summary>Box 33: Billing provider address 2</summary>
    public const string V433Addre2 = "V433ADDRE2";

    /// <summary>Box 33: Billing provider city</summary>
    public const string V433City = "V433CITY";

    /// <summary>Box 33: Billing provider state</summary>
    public const string V433State = "V433STATE";

    /// <summary>Box 33: Billing provider ZIP code</summary>
    public const string V433ZipCod = "V433ZIPCOD";

    /// <summary>Box 33: Billing provider ZIP+4</summary>
    public const string V433Plus4 = "V433PLUS4";

    /// <summary>Box 33a: Billing provider NPI</summary>
    public const string V433ANpi = "V433ANPI";

    /// <summary>Box 33b: Billing provider taxonomy code</summary>
    public const string V433BTaxon = "V433BTAXON";

    /// <summary>Box 33b: Billing provider other ID qualifier</summary>
    public const string V433BQual = "V433BQUAL";

    #endregion

    #region Service Line Helper

    /// <summary>
    /// Gets the column name for a service line field.
    /// Delegates to VdeServiceLineConstants.GetColumnName.
    /// </summary>
    /// <param name="lineNumber">Line number (1-25)</param>
    /// <param name="suffix">Field suffix from VdeServiceLineConstants.Suffix</param>
    /// <returns>Full column name (e.g., "V524ADATEF" for line 1 date from)</returns>
    public static string GetServiceLineColumn(int lineNumber, string suffix)
        => VdeServiceLineConstants.GetColumnName(lineNumber, suffix);

    #endregion

    #region Processing Constants

    /// <summary>Job type identifier for 837P transactions</summary>
    public const string Job = "TSA837P";

    /// <summary>Header identifier</summary>
    public const string Hd = "NS";

    /// <summary>Cutoff date for ICD-10 vs ICD-9 (MMDDYY format)</summary>
    public const string LimiteDateIcd10Icd9 = "093015";

    /// <summary>Starting service line view index (5 = V5)</summary>
    public const int ViewStart = 5;

    /// <summary>Ending service line view index (32 = VW, but mapped to hex)</summary>
    public const int ViewEnd = 32;

    /// <summary>Flag for ICD-10 vs ICD-9 processing</summary>
    public const bool Icd10OrIcd9 = true;

    #endregion
}
