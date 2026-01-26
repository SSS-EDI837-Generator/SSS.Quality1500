namespace SSS.Quality1500.Business.Models;

/// <summary>
/// DTO for VDE (Virtual Data Entry) records from DBF files.
/// Organized by CMS-1500 form sections for clarity.
/// </summary>
public class VdeRecordDto
{
    #region Document Identifiers

    public string? V0Document { get; set; }
    public string? V0Batchnum { get; set; }
    public string? V0Sequence { get; set; }
    public string? V0IfName01 { get; set; }
    public string? V1Page { get; set; }

    #endregion

    #region Patient Information - Box 1-4

    /// <summary>Box 1: Insurance type</summary>
    public string? V11TypeId { get; set; }

    /// <summary>Box 1a: Insured's ID number</summary>
    public string? V11AInsure { get; set; }

    /// <summary>Box 2: Patient's first name</summary>
    public string? V12Name { get; set; }

    /// <summary>Box 2: Patient's last name</summary>
    public string? V12LastName { get; set; }

    /// <summary>Box 2: Patient's middle initial</summary>
    public string? V12Initial { get; set; }

    /// <summary>Box 3: Patient's birth date</summary>
    public string? V13Birth { get; set; }

    /// <summary>Box 3: Patient's sex</summary>
    public string? V13Sexo { get; set; }

    #endregion

    #region Patient Address - Box 5

    public string? V15Address1 { get; set; }
    public string? V15Address2 { get; set; }
    public string? V15City { get; set; }
    public string? V15State { get; set; }
    public string? V15ZipCode { get; set; }
    public string? V15Plus4 { get; set; }
    public string? V15Telefon { get; set; }

    #endregion

    #region Insured Information - Box 9, 11

    public string? V29Name { get; set; }
    public string? V29LastName { get; set; }
    public string? V29APolicy { get; set; }
    public string? V29DInsPla { get; set; }
    public string? V210AEmplo { get; set; }
    public string? V210BAuto { get; set; }
    public string? V210COther { get; set; }
    public string? V211ABirth { get; set; }
    public string? V211ASexo { get; set; }
    public string? V211Insure { get; set; }

    #endregion

    #region Referring Provider - Box 17

    public string? V317Name { get; set; }
    public string? V317AQual { get; set; }
    public string? V317AReffE { get; set; }

    /// <summary>Box 17b: Referring provider NPI</summary>
    public string? V317BNpi { get; set; }

    #endregion

    #region Dates - Box 14-18

    public string? V314Qual { get; set; }
    public string? V314Date { get; set; }
    public string? V315Qual { get; set; }
    public string? V315Date { get; set; }
    public string? V316DateFr { get; set; }
    public string? V316DateTo { get; set; }
    public string? V318DateFr { get; set; }
    public string? V318DateTo { get; set; }

    #endregion

    #region Diagnosis - Box 21

    /// <summary>Box 21: ICD indicator (9=ICD-9, 0=ICD-10)</summary>
    public string? V321IcdInd { get; set; }

    /// <summary>Box 21A: Diagnosis code A</summary>
    public string? V321Diag1 { get; set; }

    /// <summary>Box 21B: Diagnosis code B</summary>
    public string? V321Diag2 { get; set; }

    /// <summary>Box 21C: Diagnosis code C</summary>
    public string? V321Diag3 { get; set; }

    /// <summary>Box 21D: Diagnosis code D</summary>
    public string? V321Diag4 { get; set; }

    /// <summary>Box 21E: Diagnosis code E</summary>
    public string? V321Diag5 { get; set; }

    /// <summary>Box 21F: Diagnosis code F</summary>
    public string? V321Diag6 { get; set; }

    /// <summary>Box 21G: Diagnosis code G</summary>
    public string? V321Diag7 { get; set; }

    /// <summary>Box 21H: Diagnosis code H</summary>
    public string? V321Diag8 { get; set; }

    /// <summary>Box 21I: Diagnosis code I</summary>
    public string? V321Diag9 { get; set; }

    /// <summary>Box 21J: Diagnosis code J</summary>
    public string? V321Diag10 { get; set; }

    /// <summary>Box 21K: Diagnosis code K</summary>
    public string? V321Diag11 { get; set; }

    /// <summary>Box 21L: Diagnosis code L</summary>
    public string? V321Diag12 { get; set; }

    #endregion

    #region Prior Authorization - Box 23

    public string? V423Prior { get; set; }

    #endregion

    #region Tax and Patient Account - Box 25-26

    public string? V425FedTax { get; set; }
    public string? V425Ssn { get; set; }
    public string? V425Ein { get; set; }
    public string? V426Patien { get; set; }

    #endregion

    #region Amounts - Box 27-30

    public string? V427Accept { get; set; }

    /// <summary>Box 28: Total charge</summary>
    public string? V428Total { get; set; }

    /// <summary>Box 29: Amount paid</summary>
    public string? V429Amount { get; set; }

    public string? V430Nucc { get; set; }

    #endregion

    #region Signature - Box 31

    public string? V431Date { get; set; }

    #endregion

    #region Service Facility - Box 32

    /// <summary>Box 32: Service facility name/address</summary>
    public string? V432Facili { get; set; }

    public string? V432ZipCod { get; set; }
    public string? V432Plus4 { get; set; }

    /// <summary>Box 32a: Service facility NPI</summary>
    public string? V432ANpi { get; set; }

    public string? V432BOther { get; set; }

    #endregion

    #region Billing Provider - Box 33

    public string? V433Name { get; set; }
    public string? V433LastNa { get; set; }
    public string? V433Addre1 { get; set; }
    public string? V433City { get; set; }
    public string? V433State { get; set; }
    public string? V433ZipCod { get; set; }
    public string? V433Plus4 { get; set; }

    /// <summary>Box 33a: Billing provider NPI</summary>
    public string? V433ANpi { get; set; }

    /// <summary>Box 33b: Taxonomy code</summary>
    public string? V433BTaxon { get; set; }

    #endregion

    #region Service Line 1 - Box 24

    /// <summary>Box 24A: Date from (line 1)</summary>
    public string? V524DateFrom { get; set; }

    /// <summary>Box 24A: Date to (line 1)</summary>
    public string? V524DateTo { get; set; }

    /// <summary>Box 24B: Place of service (line 1)</summary>
    public string? V524PlaceOfService { get; set; }

    /// <summary>Box 24D: CPT/HCPCS code (line 1)</summary>
    public string? V524Cpt { get; set; }

    /// <summary>Box 24D: Modifier 1 (line 1)</summary>
    public string? V524Mod1 { get; set; }

    /// <summary>Box 24D: Modifier 2 (line 1)</summary>
    public string? V524Mod2 { get; set; }

    /// <summary>Box 24E: Diagnosis pointer (line 1)</summary>
    public string? V524DiagPointer { get; set; }

    /// <summary>Box 24F: Charges (line 1)</summary>
    public string? V524Charges { get; set; }

    /// <summary>Box 24G: Days/units (line 1)</summary>
    public string? V524DaysUnits { get; set; }

    /// <summary>Box 24J: Rendering provider NPI (line 1)</summary>
    public string? V524RenderingNpi { get; set; }

    #endregion
}
