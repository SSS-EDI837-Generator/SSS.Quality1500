namespace SSS.Quality1500.Business.Services;

using System.Data;
using SSS.Quality1500.Common;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Mappers;
using SSS.Quality1500.Business.Services.Interfaces;

/// <summary>
/// Service for VDE record operations.
/// Wraps IDbfReader and adds VDE-specific transformations.
/// Implements Adapter/Decorator pattern.
/// </summary>
public class VdeRecordService(IDbfReader dbfReader) : IVdeRecordService
{
    public int TotalImages
    {
        get => dbfReader.TotalImages;
    }

    public int TotalClaims
    {
        get => dbfReader.TotalClaims;
    }

    public string Bht
    {
        get => dbfReader.Bht;
    }

    public Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath)
    {
        return dbfReader.GetAllAsDataTableAsync(filePath);
    }

    public DataTable ReadDbfFile(string filePath)
    {
        return dbfReader.ReadDbfFile(filePath);
    }

    public async Task<Result<List<VdeRecord>, string>> GetAllAsVdeRecordsAsync(string filePath)
    {
        Result<DataTable, string> dataTableResult = await dbfReader.GetAllAsDataTableAsync(filePath);

        return dataTableResult.Map(dataTable =>
            VdeRecordMapper.MapDataTableToVdeRecords(dataTable)
        );
    }
}
