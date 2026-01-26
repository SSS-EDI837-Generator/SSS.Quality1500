namespace SSS.Quality1500.Business.Services;

using Microsoft.Extensions.Options;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Data;

/// <summary>
/// Service for validating DBF files against the expected VDE schema.
/// </summary>
public class DbfValidationService(
    IDbfReader dbfReader,
    IOptions<DbfValidationSettings> settings) : IDbfValidationService
{
    private readonly IDbfReader _dbfReader = dbfReader;
    private readonly DbfValidationSettings _settings = settings.Value;

    public async Task<Result<DbfValidationResult, string>> ValidateDbfFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Result<DbfValidationResult, string>.Fail("La ruta del archivo no puede estar vacía.");

        if (!File.Exists(filePath))
            return Result<DbfValidationResult, string>.Fail($"El archivo no existe: {filePath}");

        try
        {
            Result<DataTable, string> readResult = await _dbfReader.GetAllAsDataTableAsync(filePath);

            if (!readResult.IsSuccess)
                return Result<DbfValidationResult, string>.Fail(readResult.GetErrorOrDefault() ?? "Error al leer el archivo DBF.");

            DataTable dataTable = readResult.GetValueOrDefault()!;

            // Get actual columns from DBF
            List<string> actualColumnsList = dataTable.Columns
                .Cast<DataColumn>()
                .Select(c => c.ColumnName.ToUpperInvariant())
                .ToList();

            HashSet<string> actualColumnsSet = [.. actualColumnsList];

            // Get expected columns
            List<string> expectedColumns = VdeConstants.GetAllExpectedColumns();

            // Find missing columns
            List<string> missingColumns = expectedColumns
                .Where(expected => !actualColumnsSet.Contains(expected.ToUpperInvariant()))
                .ToList();

            if (missingColumns.Count > 0)
            {
                return Result<DbfValidationResult, string>.Ok(new DbfValidationResult
                {
                    IsValid = false,
                    MissingColumns = missingColumns,
                    ActualColumns = actualColumnsList,
                    ErrorMessage = $"Faltan {missingColumns.Count} columnas en el archivo DBF."
                });
            }

            // Calculate totals
            int totalRecords = dataTable.Rows.Count;
            int totalClaims = CalculateTotalClaims(dataTable);

            return Result<DbfValidationResult, string>.Ok(new DbfValidationResult
            {
                IsValid = true,
                TotalRecords = totalRecords,
                TotalClaims = totalClaims
            });
        }
        catch (Exception ex)
        {
            return Result<DbfValidationResult, string>.Fail($"Error al validar el archivo: {ex.Message}");
        }
    }

    private int CalculateTotalClaims(DataTable dataTable)
    {
        string columnName = _settings.ClaimFilterColumn;
        string excludeValue = _settings.ClaimFilterExcludeValue;

        if (!dataTable.Columns.Contains(columnName))
            return dataTable.Rows.Count;

        try
        {
            DataRow[] claimRows = dataTable.Select($"{columnName} <> '{excludeValue}'");
            return claimRows.Length;
        }
        catch
        {
            return dataTable.Rows.Count;
        }
    }
}
