namespace SSS.Quality1500.Domain.Interfaces;

using System.Data;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Contrato para lectura de archivos DBF.
/// La implementacion concreta esta en la capa Data.
/// </summary>
public interface IDbfReader
{
    int TotalImages { get; }
    int TotalClaims { get; }
    string Bht { get; }

    Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath);
    DataTable ReadDbfFile(string filePath);

    /// <summary>
    /// Gets the column names from a DBF file without loading all data.
    /// </summary>
    List<string> GetColumnNames(string filePath);
}
