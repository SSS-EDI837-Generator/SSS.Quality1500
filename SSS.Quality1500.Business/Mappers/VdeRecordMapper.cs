namespace SSS.Quality1500.Business.Mappers;

using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Business.Models;
using System.Data;

public static class VdeRecordMapper
{
    public static List<VdeRecordDto> MapDataTableToVdeRecords(DataTable dataTable)
    {
        if (dataTable == null)
            return new List<VdeRecordDto>();

        var records = new List<VdeRecordDto>();

        foreach (DataRow row in dataTable.Rows)
        {
            var record = new VdeRecordDto
            {
                // Main identifiers
                V0Batchnum = GetStringValue(row, VdeConstants.V0Batchnum),
                V1Page = GetStringValue(row, VdeConstants.V1Page),
                V0IfName01 = GetStringValue(row, VdeConstants.V0IfName01),
                V0Sequence = GetStringValue(row, VdeConstants.V0Sequence),
                V0Document = GetStringValue(row, VdeConstants.V0Document),

                // Patient information
                V1Insured = GetStringValue(row, VdeConstants.V1Insured),
                V1Birthday = GetStringValue(row, VdeConstants.V1Birthday),
                V1ClaimCod = GetStringValue(row, VdeConstants.V1ClaimCod),
                V1OtherId = GetStringValue(row, VdeConstants.V1OtherId),
                V1Ispat = GetStringValue(row, VdeConstants.V1Ispat),
                V1IsThere = GetStringValue(row, VdeConstants.V1IsThere),
                V1Zua = GetStringValue(row, VdeConstants.V1Zua),
                V1Sex = GetStringValue(row, VdeConstants.V1Sex),
                V1Oth = GetStringValue(row, VdeConstants.V1Oth),
                V1Patient = GetStringValue(row, VdeConstants.V1Patient),
                V1Originan = GetStringValue(row, VdeConstants.V1Originan),

                // Diagnosis information
                V4Diag = GetStringValue(row, VdeConstants.V4Diag),
                V414Qual = GetStringValue(row, VdeConstants.V414Qual),
                V415Date = GetStringValue(row, VdeConstants.V415Date),
                V415Qual = GetStringValue(row, VdeConstants.V415Qual),
                V432Plufou = GetStringValue(row, VdeConstants.V432Plufou),
                V432Zcode = GetStringValue(row, VdeConstants.V432Zcode),
                V414Date = GetStringValue(row, VdeConstants.V414Date),

                // Dates
                V6DateFrom = GetStringValue(row, VdeConstants.V6DateFrom),
                V311Insdat = GetStringValue(row, VdeConstants.V311Insdat),
                V316Patdat = GetStringValue(row, VdeConstants.V316Patdat),
                V316Datewo = GetStringValue(row, VdeConstants.V316Datewo),
                V318Hospda = GetStringValue(row, VdeConstants.V318Hospda),
                V318Reldat = GetStringValue(row, VdeConstants.V318Reldat),

                // Provider information
                V133Anpi = GetStringValue(row, VdeConstants.V133Anpi),
                V417Bnpi = GetStringValue(row, VdeConstants.V417Bnpi),
                V417Aprove = GetStringValue(row, VdeConstants.V417Aprove),
                V133Bprovi = GetStringValue(row, VdeConstants.V133Bprovi),
                V133Bqual = GetStringValue(row, VdeConstants.V133Bqual),
                V132Anpi = GetStringValue(row, VdeConstants.V132Anpi),
                V132Bprovi = GetStringValue(row, VdeConstants.V132Bprovi),

                // Address information
                V533Add1 = GetStringValue(row, VdeConstants.V533Add1),
                V533Add2 = GetStringValue(row, VdeConstants.V533Add2),
                V533City = GetStringValue(row, VdeConstants.V533City),
                V533State = GetStringValue(row, VdeConstants.V533State),
                V533Name = GetStringValue(row, VdeConstants.V533Name),
                V533Lastn1 = GetStringValue(row, VdeConstants.V533Lastn1),
                V533Zipcoc = GetStringValue(row, VdeConstants.V533Zipcoc),
                V533Plus4 = GetStringValue(row, VdeConstants.V533Plus4),

                // Financial information
                V419Reserv = GetStringValue(row, VdeConstants.V419Reserv),
                V426Patien = GetStringValue(row, VdeConstants.V426Patien),
                V423Priot = GetStringValue(row, VdeConstants.V423Priot),
                V425Federa = GetStringValue(row, VdeConstants.V425Federa),
                V4Ssn = GetStringValue(row, VdeConstants.V4Ssn),
                V4Ein = GetStringValue(row, VdeConstants.V4Ein),
                V428Total = GetStringValue(row, VdeConstants.V428Total),
                V429Amout = GetStringValue(row, VdeConstants.V429Amout),
                V330Balanc = GetStringValue(row, VdeConstants.V330Balanc),

                // Other patient information
                V39Otherna = GetStringValue(row, VdeConstants.V39Otherna),
                V39Ini = GetStringValue(row, VdeConstants.V39Ini),
                V39Otherla = GetStringValue(row, VdeConstants.V39Otherla),
                V39Botherd = GetStringValue(row, VdeConstants.V39Botherd),
                V39Otherin = GetStringValue(row, VdeConstants.V39Otherin),
                V39Dinspla = GetStringValue(row, VdeConstants.V39Dinspla),

                // Insurance information
                V211Insure = GetStringValue(row, VdeConstants.V211Insure),
                V332Facili = GetStringValue(row, VdeConstants.V332Facili),
                V2Patstatu = GetStringValue(row, VdeConstants.V2Patstatu),

                // Charges and payments
                V320Outsid = GetStringValue(row, VdeConstants.V320Outsid),
                V320Charge = GetStringValue(row, VdeConstants.V320Charge),
                V327Accept = GetStringValue(row, VdeConstants.V327Accept),

                // Record D properties
                Datefrom = GetStringValue(row, VdeConstants.Datefrom),
                Dateto = GetStringValue(row, VdeConstants.Dateto),
                Diagpoin = GetStringValue(row, VdeConstants.Diagpoin),
                Ndc = GetStringValue(row, VdeConstants.Ndc),
                Unitqual = GetStringValue(row, VdeConstants.Unitqual),
                Unit = GetStringValue(row, VdeConstants.Unit),
                Place = GetStringValue(row, VdeConstants.Place),
                Mod1 = GetStringValue(row, VdeConstants.Mod1),
                Mod2 = GetStringValue(row, VdeConstants.Mod2),
                Mod3 = GetStringValue(row, VdeConstants.Mod3),
                Mod4 = GetStringValue(row, VdeConstants.Mod4),
                Npi = GetStringValue(row, VdeConstants.Npi),
                Cpt = GetStringValue(row, VdeConstants.Cpt),
                Rendprov = GetStringValue(row, VdeConstants.Rendprov),
                Iqual24 = GetStringValue(row, VdeConstants.Iqual24),
                Daysunit = GetStringValue(row, VdeConstants.Daysunit),
                Emg = GetStringValue(row, VdeConstants.Emg),
                Charges = GetStringValue(row, VdeConstants.Charges),
                Abbimabi = GetStringValue(row, VdeConstants.Abbimabi)
            };

            records.Add(record);
        }

        return records;
    }

    private static string GetStringValue(DataRow row, string columnName)
    {
        if (row.Table.Columns.Contains(columnName))
        {
            var value = row[columnName];
            return value?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }
}
