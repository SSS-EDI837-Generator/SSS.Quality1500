namespace SSS.Quality1500.Data.Services;

using NDbfReader;
using SSS.Quality1500.Common;
using SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

using System.Data;
using System.Text;

/// <summary>
/// Clase para leer archivos DBF y convertirlos en DataTable
/// </summary>
public class DbfReader : IDbfReader
{
    public int TotalImages { get; set; }
    public int TotalClaims { get; set; }
    public string Bht { get; set; } = string.Empty;

    static DbfReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    private static Table DbfConnection(string filename)
    {
        return Table.Open(filename);
    }

    public async Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath)
    {
        try
        {
            using Table db = DbfConnection(filePath);
            DataTable? table = await db.AsDataTableAsync();

            const string page = VdeConstants.V1Page;

            TotalImages = table?.Rows?.Count ?? 0;
            TotalClaims = table?.Select($"{page} <> '99'").Length ?? 0;
            DataRow? row = table?.Select($"{page} <> '99'")?.FirstOrDefault();
            Bht = row?[VdeConstants.V0Document].ToString()?.Substring(3, 3) ?? string.Empty;

            return Result<DataTable, string>.Ok(table ?? new DataTable());
        }
        catch (Exception ex)
        {
            return Result<DataTable, string>.Fail(ex.ToString());
        }
    }

    public DataTable ReadDbfFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"El archivo DBF no fue encontrado en la ruta: {filePath}");
        }

        var dataTable = new DataTable();
        dataTable.TableName = Path.GetFileNameWithoutExtension(filePath);

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using Table? table = Table.Open(stream);

            foreach (IColumn? column in table.Columns)
            {
                var dataColumn = new DataColumn(column.Name, column.Type);
                dataTable.Columns.Add(dataColumn);
            }

            var pathColumn = new DataColumn("VdePath", typeof(string));
            dataTable.Columns.Add(pathColumn);

            Reader? reader = table.OpenReader(Encoding.GetEncoding(1252));
            while (reader.Read())
            {
                DataRow row = dataTable.NewRow();
                foreach (IColumn? column in table.Columns)
                {
                    row[column.Name] = reader.GetValue(column) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al leer el archivo DBF: {ex.Message}", ex);
        }

        return dataTable;
    }

    public List<string> GetColumnNames(string filePath)
    {
        if (!File.Exists(filePath))
            return [];

        try
        {
            using Table table = DbfConnection(filePath);
            return table.Columns.Select(c => c.Name).ToList();
        }
        catch
        {
            return [];
        }
    }
}
