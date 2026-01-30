namespace SSS.Quality1500.Business.Queries.GetImagesFolder;

using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Data;

/// <summary>
/// Handler that reads the V0FILEPATH column from the first record of a DBF file.
/// Returns the folder path where TIF images are located.
/// </summary>
public class GetImagesFolderHandler(IDbfReader dbfReader)
    : IQueryHandler<GetImagesFolderQuery, Result<string, string>>
{
    private readonly IDbfReader _dbfReader = dbfReader;

    public async Task<Result<string, string>> HandleAsync(
        GetImagesFolderQuery query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query.DbfFilePath))
            return Result<string, string>.Fail("La ruta del archivo DBF es requerida.");

        if (!File.Exists(query.DbfFilePath))
            return Result<string, string>.Fail($"El archivo DBF no existe: {query.DbfFilePath}");

        try
        {
            Result<DataTable, string> readResult = await _dbfReader.GetAllAsDataTableAsync(query.DbfFilePath);

            if (!readResult.IsSuccess)
                return Result<string, string>.Fail(readResult.GetErrorOrDefault() ?? "Error al leer el archivo DBF.");

            DataTable dataTable = readResult.GetValueOrDefault()!;

            if (dataTable.Rows.Count == 0)
                return Result<string, string>.Fail("El archivo DBF no contiene registros.");

            if (!dataTable.Columns.Contains(VdeConstants.V0FilePath))
                return Result<string, string>.Fail($"La columna {VdeConstants.V0FilePath} no existe en el archivo DBF.");

            string? filePath = dataTable.Rows[0][VdeConstants.V0FilePath]?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(filePath))
                return Result<string, string>.Fail("La columna V0FILEPATH del primer registro está vacía.");

            return Result<string, string>.Ok(filePath);
        }
        catch (Exception ex)
        {
            return Result<string, string>.Fail($"Error al leer V0FILEPATH: {ex.Message}");
        }
    }
}
