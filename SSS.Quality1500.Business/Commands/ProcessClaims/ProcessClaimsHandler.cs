namespace SSS.Quality1500.Business.Commands.ProcessClaims;

using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Enums;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Data;

/// <summary>
/// Handler for processing claims and validating all records in a DBF file.
/// Uses policy-driven validation based on column configuration.
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
            Result<DataTable, string> readResult = await _dbfReader.GetAllAsDataTableAsync(command.DbfFilePath);

            if (!readResult.IsSuccess)
                return Result<ClaimProcessingResult, string>.Fail(readResult.GetErrorOrDefault() ?? "Error al leer el archivo DBF.");

            DataTable dataTable = readResult.GetValueOrDefault()!;

            // Build policy lookup map once
            Dictionary<string, ValidationPolicy> policyMap = BuildPolicyMap(command);

            List<RecordValidationResult> allResults = [];

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (ct.IsCancellationRequested)
                    break;

                DataRow row = dataTable.Rows[i];
                RecordValidationResult recordResult = ValidateRecord(i, row, command, policyMap);
                allResults.Add(recordResult);
            }

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

    private static Dictionary<string, ValidationPolicy> BuildPolicyMap(ProcessClaimsCommand command)
    {
        Dictionary<string, ValidationPolicy> map = new(StringComparer.Ordinal);

        foreach (ColumnValidationEntry entry in command.ValidationPolicies)
        {
            map[entry.ColumnName] = entry.Policy;
        }

        return map;
    }

    private RecordValidationResult ValidateRecord(
        int recordIndex,
        DataRow row,
        ProcessClaimsCommand command,
        Dictionary<string, ValidationPolicy> policyMap)
    {
        Dictionary<string, object?> recordData = [];
        foreach (DataColumn column in row.Table.Columns)
        {
            recordData[column.ColumnName] = row[column] == DBNull.Value ? null : row[column];
        }

        string imageFileName = recordData.TryGetValue(ImageFileNameColumn, out object? imgValue)
            ? imgValue?.ToString() ?? string.Empty
            : string.Empty;

        List<FieldValidationError> errors = [];

        foreach (string columnName in command.SelectedColumns)
        {
            if (!policyMap.TryGetValue(columnName, out ValidationPolicy? policy))
                continue;

            if (policy.Type == ValidationType.None)
                continue;

            if (!recordData.TryGetValue(columnName, out object? value))
                continue;

            FieldValidationError? error = policy.Type switch
            {
                ValidationType.Date => ValidateDate(columnName, value, policy),
                ValidationType.Icd10 => ValidateIcd10(columnName, value, policy),
                ValidationType.Required => ValidateRequired(columnName, value),
                ValidationType.Npi => null,     // TODO: Implement when API is available
                ValidationType.Member => null,  // TODO: Implement when API is available
                _ => null
            };

            if (error is not null)
                errors.Add(error);
        }

        return errors.Count > 0
            ? RecordValidationResult.WithErrors(recordIndex, imageFileName, recordData, errors)
            : RecordValidationResult.Valid(recordIndex, imageFileName, recordData);
    }

    private static FieldValidationError? ValidateDate(string columnName, object? value, ValidationPolicy policy)
    {
        bool allowEmpty = GetBoolOption(policy, "AllowEmpty", true);
        bool allowFuture = GetBoolOption(policy, "AllowFuture", false);

        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            if (allowEmpty)
                return null;

            return FieldValidationError.Required(columnName, GetDisplayName(columnName));
        }

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

        if (!allowFuture && dateValue.Date > DateTime.Today)
            return FieldValidationError.FutureDate(columnName, GetDisplayName(columnName), value);

        return null;
    }

    private FieldValidationError? ValidateIcd10(string columnName, object? value, ValidationPolicy policy)
    {
        bool allowEmpty = GetBoolOption(policy, "AllowEmpty", true);

        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            if (allowEmpty)
                return null;

            return FieldValidationError.Required(columnName, GetDisplayName(columnName));
        }

        string code = value.ToString()!.Trim();

        if (!_icd10Repository.IsValidCode(code))
            return FieldValidationError.InvalidCode(columnName, GetDisplayName(columnName), code, "ICD-10");

        return null;
    }

    private static FieldValidationError? ValidateRequired(string columnName, object? value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return FieldValidationError.Required(columnName, GetDisplayName(columnName));

        return null;
    }

    private static bool GetBoolOption(ValidationPolicy policy, string key, bool defaultValue)
    {
        if (!policy.Options.TryGetValue(key, out string? val))
            return defaultValue;

        if (bool.TryParse(val, out bool parsed))
            return parsed;

        return defaultValue;
    }

    private static string GetDisplayName(string columnName)
    {
        return columnName switch
        {
            "V0FECSER" => "Fecha de Servicio",
            "V0FECINI" => "Fecha Inicio",
            "V0FECFIN" => "Fecha Fin",
            "V0FECNAC" => "Fecha Nacimiento",
            "V0FECAUT" => "Fecha Autorizacion",
            "V0FECING" => "Fecha Ingreso",
            "V0FECALTA" => "Fecha Alta",
            "V0ICD101" => "Diagnóstico 1",
            "V0ICD102" => "Diagnóstico 2",
            "V0ICD103" => "Diagnóstico 3",
            "V0ICD104" => "Diagnóstico 4",
            "V0ICD105" => "Diagnóstico 5",
            "V0ICD106" => "Diagnóstico 6",
            "V0ICD107" => "Diagnóstico 7",
            "V0ICD108" => "Diagnóstico 8",
            "V317BNPI" => "NPI Referidor",
            "V432ANPI" => "NPI Facilidad",
            "V433ANPI" => "NPI Facturacion",
            "V11AINSURE" => "ID Asegurado",
            _ => columnName
        };
    }
}
