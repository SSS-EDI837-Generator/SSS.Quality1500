namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

/// <summary>
/// Repository for ICD-10 code validation.
/// </summary>
public interface IIcd10Repository
{
    /// <summary>
    /// Checks if the given ICD-10 code is valid.
    /// </summary>
    bool IsValidCode(string code);

    /// <summary>
    /// Gets the description for a valid ICD-10 code.
    /// </summary>
    string? GetDescription(string code);

    /// <summary>
    /// Gets the total number of codes loaded.
    /// </summary>
    int TotalCodes { get; }

    /// <summary>
    /// Adds a new ICD-10 code to the catalog.
    /// </summary>
    /// <returns>True if added, false if the code already exists.</returns>
    bool AddCode(string code, string description);

    /// <summary>
    /// Removes an ICD-10 code from the catalog.
    /// </summary>
    /// <returns>True if removed, false if the code was not found.</returns>
    bool RemoveCode(string code);

    /// <summary>
    /// Searches codes by code or description (case-insensitive).
    /// </summary>
    List<Icd10CodeEntry> SearchCodes(string searchTerm, int maxResults = 100);

    /// <summary>
    /// Persists current state to the JSON file.
    /// </summary>
    Result<int, string> SaveChanges();
}
