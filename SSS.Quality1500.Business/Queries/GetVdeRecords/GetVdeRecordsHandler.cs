namespace SSS.Quality1500.Business.Queries.GetVdeRecords;

using SSS.Quality1500.Business.Mappers;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Data;

/// <summary>
/// Handler for retrieving VDE records from a DBF file.
/// </summary>
public class GetVdeRecordsHandler(IDbfReader dbfReader)
    : IQueryHandler<GetVdeRecordsQuery, Result<List<VdeRecordDto>, string>>
{
    private readonly IDbfReader _dbfReader = dbfReader;

    public async Task<Result<List<VdeRecordDto>, string>> HandleAsync(
        GetVdeRecordsQuery query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query.FilePath))
            return Result<List<VdeRecordDto>, string>.Fail("La ruta del archivo no puede estar vacía.");

        if (!File.Exists(query.FilePath))
            return Result<List<VdeRecordDto>, string>.Fail($"El archivo no existe: {query.FilePath}");

        Result<DataTable, string> dataTableResult = await _dbfReader.GetAllAsDataTableAsync(query.FilePath);

        return dataTableResult.Map(dataTable =>
            VdeRecordMapper.MapDataTableToVdeRecords(dataTable));
    }
}
