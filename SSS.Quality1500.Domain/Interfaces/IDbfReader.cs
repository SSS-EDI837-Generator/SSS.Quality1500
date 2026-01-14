namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Common;
using System.Data;

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
}
