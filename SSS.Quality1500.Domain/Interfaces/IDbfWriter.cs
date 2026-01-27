namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

/// <summary>
/// Contrato para escritura de archivos DBF.
/// La implementacion concreta esta en la capa Data.
/// </summary>
public interface IDbfWriter
{
    /// <summary>
    /// Updates a single record in a DBF file.
    /// </summary>
    /// <param name="filePath">Path to the DBF file.</param>
    /// <param name="recordIndex">Zero-based index of the record to update.</param>
    /// <param name="fieldValues">Dictionary of field names and their new values.</param>
    /// <returns>Result indicating success or failure with error message.</returns>
    Result<bool, string> UpdateRecord(string filePath, int recordIndex, Dictionary<string, object?> fieldValues);

    /// <summary>
    /// Updates multiple records in a DBF file.
    /// </summary>
    /// <param name="filePath">Path to the DBF file.</param>
    /// <param name="updates">List of record updates (index and field values).</param>
    /// <returns>Result indicating success or failure with error message.</returns>
    Result<int, string> UpdateRecords(string filePath, List<(int RecordIndex, Dictionary<string, object?> FieldValues)> updates);

    /// <summary>
    /// Marks a record as deleted in a DBF file (sets deletion flag).
    /// </summary>
    /// <param name="filePath">Path to the DBF file.</param>
    /// <param name="recordIndex">Zero-based index of the record to mark as deleted.</param>
    /// <returns>Result indicating success or failure with error message.</returns>
    Result<bool, string> MarkRecordDeleted(string filePath, int recordIndex);
}
