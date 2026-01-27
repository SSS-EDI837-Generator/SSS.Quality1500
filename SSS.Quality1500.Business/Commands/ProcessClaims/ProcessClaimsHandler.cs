namespace SSS.Quality1500.Business.Commands.ProcessClaims;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Data;

/// <summary>
/// Handler for processing claims and validating all records in a DBF file.
/// Orchestrates date and ICD-10 validations for selected columns.
/// </summary>
public class ProcessClaimsHandler(
    IDbfReader dbfReader,
    IIcd10Repository icd10Repository) : ICommandHandler<ProcessClaimsCommand, Result<ClaimProcessingResult, string>>
{
    private readonly IDbfReader _dbfReader = dbfReader;
    private readonly IIcd10Repository _icd10Repository = icd10Repository;

    private const string ImageFileNameColumn = "V0IFNAME01";

    public async Task<Result<ClaimProcessingResult, string>> HandleAsync(
        ProcessClaimsCommand command, CancellationToken ct = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(command.DbfFilePath))
            return Result<ClaimProcessingResult, string>.Fail("La ruta del archivo DBF es requerida.");

        if (!File.Exists(command.DbfFilePath))
            return Result<ClaimProcessingResult, string>.Fail($"El archivo DBF no existe: {command.DbfFilePath}");

        if (string.IsNullOrWhiteSpace(command.ImagesFolderPath))
            return Result<ClaimProcessingResult, string>.Fail("La ruta de la carpeta de imágenes es requerida.");

        if (!Directory.Exists(command.ImagesFolderPath))
            return Result<ClaimProcessingResult, string>.Fail($"La carpeta de imágenes no existe: {command.ImagesFolderPath}");

        try
        {
            // Read DBF file
            Result<DataTable, string> readResult = await _dbfReader.GetAllAsDataTableAsync(command.DbfFilePath);

            if (!readResult.IsSuccess)
                return Result<ClaimProcessingResult, string>.Fail(readResult.GetErrorOrDefault() ?? "Error al leer el archivo DBF.");

            DataTable dataTable = readResult.GetValueOrDefault()!;

            // Process each record
            List<RecordValidationResult> allResults = [];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (ct.IsCancellationRequested)
                    break;

                DataRow row = dataTable.Rows[i];
                RecordValidationResult recordResult = ValidateRecord(i, row, command);
                allResults.Add(recordResult);
            }

            // Create processing result
            ClaimProcessingResult result = ClaimProcessingResult.FromValidationResults(
                command.DbfFilePath,
                command.ImagesFolderPath,
                allResults);

            return Result<ClaimProcessingResult, string>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<ClaimProcessingResult, string>.Fail($"Error durante el procesamiento: {ex.Message}");
        }
    }

    private RecordValidationResult ValidateRecord(int recordIndex, DataRow row, ProcessClaimsCommand command)
    {
        // Extract record data
        Dictionary<string, object?> recordData = [];
        foreach (DataColumn column in row.Table.Columns)
        {
            recordData[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
        }

        // Get image filename
        string imageFileName = recordData.TryGetValue(ImageFileNameColumn, out object? imgValue)
            ? imgValue?.ToString() ?? string.Empty
            : string.Empty;

        // Collect validation errors
        List<FieldValidationError> errors = [];

        // Validate date columns
        foreach (string dateColumn in command.DateColumns)
        {
            if (!command.SelectedColumns.Contains(dateColumn))
                continue;

            if (!recordData.TryGetValue(dateColumn, out object? dateValue))
                continue;

            FieldValidationError? error = ValidateDate(dateColumn, dateValue);
            if (error is not null)
                errors.Add(error);
        }

        // Validate ICD-10 columns
        foreach (string icd10Column in command.Icd10Columns)
        {
            if (!command.SelectedColumns.Contains(icd10Column))
                continue;

            if (!recordData.TryGetValue(icd10Column, out object? icd10Value))
                continue;

            FieldValidationError? error = ValidateIcd10(icd10Column, icd10Value);
            if (error is not null)
                errors.Add(error);
        }

        return errors.Count > 0
            ? RecordValidationResult.WithErrors(recordIndex, imageFileName, recordData, errors)
            : RecordValidationResult.Valid(recordIndex, imageFileName, recordData);
    }

    private static FieldValidationError? ValidateDate(string columnName, object? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return null; // Empty dates are allowed for now

        DateTime dateValue;

        if (value is DateTime dt)
        {
            dateValue = dt;
        }
        else if (!DateTime.TryParse(value.ToString(), out dateValue))
        {
            return FieldValidationError.InvalidFormat(
                columnName,
                GetDisplayName(columnName),
                value,
                $"Formato de fecha inválido: {value}");
        }

        // Check for future date
        if (dateValue.Date > DateTime.Today)
        {
            return FieldValidationError.FutureDate(columnName, GetDisplayName(columnName), value);
        }

        return null;
    }

    private FieldValidationError? ValidateIcd10(string columnName, object? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return null; // Empty ICD-10 codes are allowed for now

        string code = value.ToString()!.Trim();

        if (!_icd10Repository.IsValidCode(code))
        {
            return FieldValidationError.InvalidCode(columnName, GetDisplayName(columnName), code, "ICD-10");
        }

        return null;
    }

    private static string GetDisplayName(string columnName)
    {
        // Map DBF column names to user-friendly display names
        // This could be moved to a configuration or constants file
        return columnName switch
        {
            "V0FECSER" => "Fecha de Servicio",
            "V0FECINI" => "Fecha Inicio",
            "V0FECFIN" => "Fecha Fin",
            "V0FECNAC" => "Fecha Nacimiento",
            "V0ICD101" => "Diagnóstico 1",
            "V0ICD102" => "Diagnóstico 2",
            "V0ICD103" => "Diagnóstico 3",
            "V0ICD104" => "Diagnóstico 4",
            _ => columnName
        };
    }
}
