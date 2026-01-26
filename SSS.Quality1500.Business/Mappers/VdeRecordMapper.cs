namespace SSS.Quality1500.Business.Mappers;

using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Business.Models;
using System.Data;

/// <summary>
/// Maps DataTable rows from DBF files to VdeRecordDto objects.
/// Uses VdeConstants for column name references.
/// </summary>
public static class VdeRecordMapper
{
    public static List<VdeRecordDto> MapDataTableToVdeRecords(DataTable dataTable)
    {
        if (dataTable == null)
            return [];

        List<VdeRecordDto> records = [];

        foreach (DataRow row in dataTable.Rows)
        {
            VdeRecordDto record = new()
            {
                // Document identifiers
                V0Document = GetStringValue(row, VdeConstants.V0Document),
                V0Batchnum = GetStringValue(row, VdeConstants.V0Batchnum),
                V0Sequence = GetStringValue(row, VdeConstants.V0Sequence),
                V0IfName01 = GetStringValue(row, VdeConstants.V0IfName01),
                V1Page = GetStringValue(row, VdeConstants.V1Page),

                // Patient information - Box 1-4
                V11TypeId = GetStringValue(row, VdeConstants.V11TypeId),
                V11AInsure = GetStringValue(row, VdeConstants.V11AInsure),
                V12Name = GetStringValue(row, VdeConstants.V12Name),
                V12LastName = GetStringValue(row, VdeConstants.V12LastName),
                V12Initial = GetStringValue(row, VdeConstants.V12Initial),
                V13Birth = GetStringValue(row, VdeConstants.V13Birth),
                V13Sexo = GetStringValue(row, VdeConstants.V13Sexo),

                // Patient address - Box 5
                V15Address1 = GetStringValue(row, VdeConstants.V15Address1),
                V15Address2 = GetStringValue(row, VdeConstants.V15Address2),
                V15City = GetStringValue(row, VdeConstants.V15City),
                V15State = GetStringValue(row, VdeConstants.V15State),
                V15ZipCode = GetStringValue(row, VdeConstants.V15ZipCode),
                V15Plus4 = GetStringValue(row, VdeConstants.V15Plus4),
                V15Telefon = GetStringValue(row, VdeConstants.V15Telefon),

                // Insured information - Box 9, 11
                V29Name = GetStringValue(row, VdeConstants.V29Name),
                V29LastName = GetStringValue(row, VdeConstants.V29LastName),
                V29APolicy = GetStringValue(row, VdeConstants.V29APolicy),
                V29DInsPla = GetStringValue(row, VdeConstants.V29DInsPla),
                V210AEmplo = GetStringValue(row, VdeConstants.V210AEmplo),
                V210BAuto = GetStringValue(row, VdeConstants.V210BAuto),
                V210COther = GetStringValue(row, VdeConstants.V210COther),
                V211ABirth = GetStringValue(row, VdeConstants.V211ABirth),
                V211ASexo = GetStringValue(row, VdeConstants.V211ASexo),
                V211Insure = GetStringValue(row, VdeConstants.V211Insure),

                // Referring provider - Box 17
                V317Name = GetStringValue(row, VdeConstants.V317Name),
                V317AQual = GetStringValue(row, VdeConstants.V317AQual),
                V317AReffE = GetStringValue(row, VdeConstants.V317AReffE),
                V317BNpi = GetStringValue(row, VdeConstants.V317BNpi),

                // Dates - Box 14-18
                V314Qual = GetStringValue(row, VdeConstants.V314Qual),
                V314Date = GetStringValue(row, VdeConstants.V314Date),
                V315Qual = GetStringValue(row, VdeConstants.V315Qual),
                V315Date = GetStringValue(row, VdeConstants.V315Date),
                V316DateFr = GetStringValue(row, VdeConstants.V316DateFr),
                V316DateTo = GetStringValue(row, VdeConstants.V316DateTo),
                V318DateFr = GetStringValue(row, VdeConstants.V318DateFr),
                V318DateTo = GetStringValue(row, VdeConstants.V318DateTo),

                // Diagnosis - Box 21
                V321IcdInd = GetStringValue(row, VdeConstants.V321IcdInd),
                V321Diag1 = GetStringValue(row, VdeConstants.V321Diag1),
                V321Diag2 = GetStringValue(row, VdeConstants.V321Diag2),
                V321Diag3 = GetStringValue(row, VdeConstants.V321Diag3),
                V321Diag4 = GetStringValue(row, VdeConstants.V321Diag4),
                V321Diag5 = GetStringValue(row, VdeConstants.V321Diag5),
                V321Diag6 = GetStringValue(row, VdeConstants.V321Diag6),
                V321Diag7 = GetStringValue(row, VdeConstants.V321Diag7),
                V321Diag8 = GetStringValue(row, VdeConstants.V321Diag8),
                V321Diag9 = GetStringValue(row, VdeConstants.V321Diag9),
                V321Diag10 = GetStringValue(row, VdeConstants.V321Diag10),
                V321Diag11 = GetStringValue(row, VdeConstants.V321Diag11),
                V321Diag12 = GetStringValue(row, VdeConstants.V321Diag12),

                // Prior authorization - Box 23
                V423Prior = GetStringValue(row, VdeConstants.V423Prior),

                // Tax and patient account - Box 25-26
                V425FedTax = GetStringValue(row, VdeConstants.V425FedTax),
                V425Ssn = GetStringValue(row, VdeConstants.V425Ssn),
                V425Ein = GetStringValue(row, VdeConstants.V425Ein),
                V426Patien = GetStringValue(row, VdeConstants.V426Patien),

                // Amounts - Box 27-30
                V427Accept = GetStringValue(row, VdeConstants.V427Accept),
                V428Total = GetStringValue(row, VdeConstants.V428Total),
                V429Amount = GetStringValue(row, VdeConstants.V429Amount),
                V430Nucc = GetStringValue(row, VdeConstants.V430Nucc),

                // Signature - Box 31
                V431Date = GetStringValue(row, VdeConstants.V431Date),

                // Service facility - Box 32
                V432Facili = GetStringValue(row, VdeConstants.V432Facili),
                V432ZipCod = GetStringValue(row, VdeConstants.V432ZipCod),
                V432Plus4 = GetStringValue(row, VdeConstants.V432Plus4),
                V432ANpi = GetStringValue(row, VdeConstants.V432ANpi),
                V432BOther = GetStringValue(row, VdeConstants.V432BOther),

                // Billing provider - Box 33
                V433Name = GetStringValue(row, VdeConstants.V433Name),
                V433LastNa = GetStringValue(row, VdeConstants.V433LastNa),
                V433Addre1 = GetStringValue(row, VdeConstants.V433Addre1),
                V433City = GetStringValue(row, VdeConstants.V433City),
                V433State = GetStringValue(row, VdeConstants.V433State),
                V433ZipCod = GetStringValue(row, VdeConstants.V433ZipCod),
                V433Plus4 = GetStringValue(row, VdeConstants.V433Plus4),
                V433ANpi = GetStringValue(row, VdeConstants.V433ANpi),
                V433BTaxon = GetStringValue(row, VdeConstants.V433BTaxon),

                // Service line 1 - Box 24 (example for first line)
                V524DateFrom = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.DateFrom)),
                V524DateTo = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.DateTo)),
                V524PlaceOfService = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.PlaceOfService)),
                V524Cpt = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.Cpt)),
                V524Mod1 = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.Mod1)),
                V524Mod2 = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.Mod2)),
                V524DiagPointer = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.DiagPointer)),
                V524Charges = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.Charges)),
                V524DaysUnits = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.DaysUnits)),
                V524RenderingNpi = GetStringValue(row, VdeServiceLineConstants.GetColumnName(1, VdeServiceLineConstants.Suffix.RenderingNpi)),
            };

            records.Add(record);
        }

        return records;
    }

    private static string GetStringValue(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName))
            return string.Empty;

        object? value = row[columnName];
        return value?.ToString() ?? string.Empty;
    }
}
