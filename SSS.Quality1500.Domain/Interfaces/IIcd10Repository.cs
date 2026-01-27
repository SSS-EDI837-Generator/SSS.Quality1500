namespace SSS.Quality1500.Domain.Interfaces;

/// <summary>
/// Repository for ICD-10 code validation.
/// </summary>
public interface IIcd10Repository
{
    /// <summary>
    /// Checks if the given ICD-10 code is valid.
    /// </summary>
    /// <param name="code">The ICD-10 code to validate.</param>
    /// <returns>True if the code is valid, false otherwise.</returns>
    bool IsValidCode(string code);

    /// <summary>
    /// Gets the description for a valid ICD-10 code.
    /// </summary>
    /// <param name="code">The ICD-10 code.</param>
    /// <returns>The description, or null if the code is not found.</returns>
    string? GetDescription(string code);

    /// <summary>
    /// Gets the total number of codes loaded.
    /// </summary>
    int TotalCodes { get; }
}
