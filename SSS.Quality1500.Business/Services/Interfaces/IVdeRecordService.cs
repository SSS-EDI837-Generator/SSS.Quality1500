using System.Data;
using SSS.Quality1500.Common;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Domain.Models;

namespace SSS.Quality1500.Business.Services.Interfaces;

/// <summary>
/// Service contract for VDE record operations.
/// Extends DBF reading capabilities with VDE-specific transformations.
/// </summary>
public interface IVdeRecordService
{
    /// <summary>
    /// Total number of images in the DBF file
    /// </summary>
    int TotalImages { get; }

    /// <summary>
    /// Total number of claims in the DBF file
    /// </summary>
    int TotalClaims { get; }

    /// <summary>
    /// Batch header information
    /// </summary>
    string Bht { get; }

    /// <summary>
    /// Reads DBF file and returns as DataTable
    /// </summary>
    Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath);

    /// <summary>
    /// Reads DBF file without async
    /// </summary>
    DataTable ReadDbfFile(string filePath);

    /// <summary>
    /// Reads DBF file and maps to VDE record DTOs
    /// </summary>
    Task<Result<List<VdeRecordDto>, string>> GetAllAsVdeRecordsAsync(string filePath);
}
