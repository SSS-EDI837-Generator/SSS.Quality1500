namespace SSS.Quality1500.Business.Models;
// Esto es un ejemplo de DTO para los registros VDE
public class VdeRecordDto
{
    // Main identifiers
    public string? V0Batchnum { get; set; }
    public string? V1Page { get; set; }
    public string? V0IfName01 { get; set; }
    public string? V0Sequence { get; set; }
    public string? V0Document { get; set; }
    
    // Patient information
    public string? V1Insured { get; set; }
    public string? V1Birthday { get; set; }
    public string? V1ClaimCod { get; set; }
    public string? V1OtherId { get; set; }
    public string? V1Ispat { get; set; }
    public string? V1IsThere { get; set; }
    public string? V1Zua { get; set; }
    public string? V1Sex { get; set; }
    public string? V1Oth { get; set; }
    public string? V1Patient { get; set; }
    public string? V1Originan { get; set; }
    
    // Diagnosis information
    public string? V4Diag { get; set; }
    public string? V414Qual { get; set; }
    public string? V415Date { get; set; }
    public string? V415Qual { get; set; }
    public string? V432Plufou { get; set; }
    public string? V432Zcode { get; set; }
    public string? V414Date { get; set; }
    
    // Dates
    public string? V6DateFrom { get; set; }
    public string? V311Insdat { get; set; }
    public string? V316Patdat { get; set; }
    public string? V316Datewo { get; set; }
    public string? V318Hospda { get; set; }
    public string? V318Reldat { get; set; }
    
    // Provider information
    public string? V133Anpi { get; set; }
    public string? V417Bnpi { get; set; }
    public string? V417Aprove { get; set; }
    public string? V133Bprovi { get; set; }
    public string? V133Bqual { get; set; }
    public string? V132Anpi { get; set; }
    public string? V132Bprovi { get; set; }
    
    // Address information
    public string? V533Add1 { get; set; }
    public string? V533Add2 { get; set; }
    public string? V533City { get; set; }
    public string? V533State { get; set; }
    public string? V533Name { get; set; }
    public string? V533Lastn1 { get; set; }
    // public string? V533Lastn2 { get; set; }
    public string? V533Zipcoc { get; set; }
    public string? V533Plus4 { get; set; }
    
    // Financial information
    public string? V419Reserv { get; set; }
    public string? V426Patien { get; set; }
    public string? V423Priot { get; set; }
    public string? V425Federa { get; set; }
    public string? V4Ssn { get; set; }
    public string? V4Ein { get; set; }
    public string? V428Total { get; set; }
    public string? V429Amout { get; set; }
    public string? V330Balanc { get; set; }
    
    // Other patient information
    public string? V39Otherna { get; set; }
    public string? V39Ini { get; set; }
    public string? V39Otherla { get; set; }
    // public string? V39Bsex { get; set; }
    public string? V39Botherd { get; set; }
    public string? V39Otherin { get; set; }
    public string? V39Dinspla { get; set; }
    
    // Insurance information
    public string? V211Insure { get; set; }
   public string? V332Facili { get; set; }

    public string? V2Patstatu { get; set; }
    // public string? V38Empl { get; set; }
    
    // Charges and payments
    public string? V320Outsid { get; set; }
    public string? V320Charge { get; set; }
    public string? V327Accept { get; set; }
    
    // Record D properties
    public string? Datefrom { get; set; }
    public string? Dateto { get; set; }
    public string? Diagpoin { get; set; }
    public string? Ndc { get; set; }
    public string? Unitqual { get; set; }
    public string? Unit { get; set; }
    public string? Place { get; set; }
    public string? Mod1 { get; set; }
    public string? Mod2 { get; set; }
    public string? Mod3 { get; set; }
    public string? Mod4 { get; set; }
    public string? Npi { get; set; }
    public string? Cpt { get; set; }
    public string? Rendprov { get; set; }
    public string? Iqual24 { get; set; }
    public string? Daysunit { get; set; }
    public string? Emg { get; set; }
    public string? Charges { get; set; }
    public string? Abbimabi { get; set; }
}
