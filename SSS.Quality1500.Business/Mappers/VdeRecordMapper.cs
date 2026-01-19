namespace SSS.Quality1500.Business.Mappers;

using SSS.Quality1500.Common.Constants;
using SSS.Quality1500.Business.Models;
using System.Data;

public static class VdeRecordMapper
{
    public static List<VdeRecord> MapDataTableToVdeRecords(DataTable dataTable)
    {
        if (dataTable == null)
            return new List<VdeRecord>();

        var records = new List<VdeRecord>();

        foreach (DataRow row in dataTable.Rows)
        {
            var record = new VdeRecord
            {
                // Main identifiers
                V0Batchnum = GetStringValue(row, VdeConstantsUpdated.V0Batchnum),
                V1Page = GetStringValue(row, VdeConstantsUpdated.V1Page),
                V0IfName01 = GetStringValue(row, VdeConstantsUpdated.V0IfName01),
                V0Sequence = GetStringValue(row, VdeConstantsUpdated.V0Sequence),
                V0Document = GetStringValue(row, VdeConstantsUpdated.V0Document),

                // Patient information
                V1Insured = GetStringValue(row, VdeConstantsUpdated.V1Insured),
                V1Birthday = GetStringValue(row, VdeConstantsUpdated.V1Birthday),
                V1ClaimCod = GetStringValue(row, VdeConstantsUpdated.V1ClaimCod),
                V1OtherId = GetStringValue(row, VdeConstantsUpdated.V1OtherId),
                V1Ispat = GetStringValue(row, VdeConstantsUpdated.V1Ispat),
                V1IsThere = GetStringValue(row, VdeConstantsUpdated.V1IsThere),
                V1Zua = GetStringValue(row, VdeConstantsUpdated.V1Zua),
                V1Sex = GetStringValue(row, VdeConstantsUpdated.V1Sex),
                V1Oth = GetStringValue(row, VdeConstantsUpdated.V1Oth),
                V1Patient = GetStringValue(row, VdeConstantsUpdated.V1Patient),
                V1Originan = GetStringValue(row, VdeConstantsUpdated.V1Originan),

                // Diagnosis information
                V4Diag = GetStringValue(row, VdeConstantsUpdated.V4Diag),
                V414Qual = GetStringValue(row, VdeConstantsUpdated.V414Qual),
                V415Date = GetStringValue(row, VdeConstantsUpdated.V415Date),
                V415Qual = GetStringValue(row, VdeConstantsUpdated.V415Qual),
                V432Plufou = GetStringValue(row, VdeConstantsUpdated.V432Plufou),
                V432Zcode = GetStringValue(row, VdeConstantsUpdated.V432Zcode),
                V414Date = GetStringValue(row, VdeConstantsUpdated.V414Date),

                // Dates
                V6DateFrom = GetStringValue(row, VdeConstantsUpdated.V6DateFrom),
                V311Insdat = GetStringValue(row, VdeConstantsUpdated.V311Insdat),
                V316Patdat = GetStringValue(row, VdeConstantsUpdated.V316Patdat),
                V316Datewo = GetStringValue(row, VdeConstantsUpdated.V316Datewo),
                V318Hospda = GetStringValue(row, VdeConstantsUpdated.V318Hospda),
                V318Reldat = GetStringValue(row, VdeConstantsUpdated.V318Reldat),

                // Provider information
                V133Anpi = GetStringValue(row, VdeConstantsUpdated.V133Anpi),
                V417Bnpi = GetStringValue(row, VdeConstantsUpdated.V417Bnpi),
                V417Aprove = GetStringValue(row, VdeConstantsUpdated.V417Aprove),
                V133Bprovi = GetStringValue(row, VdeConstantsUpdated.V133Bprovi),
                V133Bqual = GetStringValue(row, VdeConstantsUpdated.V133Bqual),
                V132Anpi = GetStringValue(row, VdeConstantsUpdated.V132Anpi),
                V132Bprovi = GetStringValue(row, VdeConstantsUpdated.V132Bprovi),

                // Address information
                V533Add1 = GetStringValue(row, VdeConstantsUpdated.V533Add1),
                V533Add2 = GetStringValue(row, VdeConstantsUpdated.V533Add2),
                V533City = GetStringValue(row, VdeConstantsUpdated.V533City),
                V533State = GetStringValue(row, VdeConstantsUpdated.V533State),
                V533Name = GetStringValue(row, VdeConstantsUpdated.V533Name),
                V533Lastn1 = GetStringValue(row, VdeConstantsUpdated.V533Lastn1),
                // V533Lastn2 = GetStringValue(row, VdeConstantsUpdated.V533Lastn2),
                V533Zipcoc = GetStringValue(row, VdeConstantsUpdated.V533Zipcoc),
                V533Plus4 = GetStringValue(row, VdeConstantsUpdated.V533Plus4),

                // Financial information
                V419Reserv = GetStringValue(row, VdeConstantsUpdated.V419Reserv),
                V426Patien = GetStringValue(row, VdeConstantsUpdated.V426Patien),
                V423Priot = GetStringValue(row, VdeConstantsUpdated.V423Priot),
                V425Federa = GetStringValue(row, VdeConstantsUpdated.V425Federa),
                V4Ssn = GetStringValue(row, VdeConstantsUpdated.V4Ssn),
                V4Ein = GetStringValue(row, VdeConstantsUpdated.V4Ein),
                V428Total = GetStringValue(row, VdeConstantsUpdated.V428Total),
                V429Amout = GetStringValue(row, VdeConstantsUpdated.V429Amout),
                V330Balanc = GetStringValue(row, VdeConstantsUpdated.V330Balanc),

                // Other patient information
                V39Otherna = GetStringValue(row, VdeConstantsUpdated.V39Otherna),
                V39Ini = GetStringValue(row, VdeConstantsUpdated.V39Ini),
                V39Otherla = GetStringValue(row, VdeConstantsUpdated.V39Otherla),
                // V39Bsex = GetStringValue(row, VdeConstantsUpdated.V39Bsex),
                V39Botherd = GetStringValue(row, VdeConstantsUpdated.V39Botherd),
                V39Otherin = GetStringValue(row, VdeConstantsUpdated.V39Otherin),
                V39Dinspla = GetStringValue(row, VdeConstantsUpdated.V39Dinspla),

                // Insurance information
                V211Insure = GetStringValue(row, VdeConstantsUpdated.V211Insure),
                V332Facili = GetStringValue(row, VdeConstantsUpdated.V332Facili),
                V2Patstatu = GetStringValue(row, VdeConstantsUpdated.V2Patstatu),
                // V38Empl = GetStringValue(row, VdeConstantsUpdated.V38Empl),

                // Charges and payments
                V320Outsid = GetStringValue(row, VdeConstantsUpdated.V320Outsid),
                V320Charge = GetStringValue(row, VdeConstantsUpdated.V320Charge),
                V327Accept = GetStringValue(row, VdeConstantsUpdated.V327Accept),

                // Record D properties
                Datefrom = GetStringValue(row, VdeConstantsUpdated.Datefrom),
                Dateto = GetStringValue(row, VdeConstantsUpdated.Dateto),
                Diagpoin = GetStringValue(row, VdeConstantsUpdated.Diagpoin),
                Ndc = GetStringValue(row, VdeConstantsUpdated.Ndc),
                Unitqual = GetStringValue(row, VdeConstantsUpdated.Unitqual),
                Unit = GetStringValue(row, VdeConstantsUpdated.Unit),
                Place = GetStringValue(row, VdeConstantsUpdated.Place),
                Mod1 = GetStringValue(row, VdeConstantsUpdated.Mod1),
                Mod2 = GetStringValue(row, VdeConstantsUpdated.Mod2),
                Mod3 = GetStringValue(row, VdeConstantsUpdated.Mod3),
                Mod4 = GetStringValue(row, VdeConstantsUpdated.Mod4),
                Npi = GetStringValue(row, VdeConstantsUpdated.Npi),
                Cpt = GetStringValue(row, VdeConstantsUpdated.Cpt),
                Rendprov = GetStringValue(row, VdeConstantsUpdated.Rendprov),
                Iqual24 = GetStringValue(row, VdeConstantsUpdated.Iqual24),
                Daysunit = GetStringValue(row, VdeConstantsUpdated.Daysunit),
                Emg = GetStringValue(row, VdeConstantsUpdated.Emg),
                Charges = GetStringValue(row, VdeConstantsUpdated.Charges),
                Abbimabi = GetStringValue(row, VdeConstantsUpdated.Abbimabi)
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
