namespace SSS.Quality1500.Data.Repositories;

using SSS.Quality1500.Domain.Interfaces;
using System.Reflection;
using System.Text.Json;

/// <summary>
/// Repository for ICD-10 code validation using embedded JSON resource.
/// </summary>
public class Icd10Repository : IIcd10Repository
{
    private readonly Dictionary<string, string> _codes;
    private readonly HashSet<string> _normalizedCodes;

    public Icd10Repository()
    {
        _codes = LoadCodesFromResource();
        _normalizedCodes = new HashSet<string>(
            _codes.Keys.Select(NormalizeCode),
            StringComparer.OrdinalIgnoreCase);
    }

    public int TotalCodes => _codes.Count;

    public bool IsValidCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        string normalized = NormalizeCode(code);
        return _normalizedCodes.Contains(normalized);
    }

    public string? GetDescription(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        string normalized = NormalizeCode(code);

        // Try to find exact match first
        foreach (KeyValuePair<string, string> kvp in _codes)
        {
            if (NormalizeCode(kvp.Key).Equals(normalized, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        return null;
    }

    private static string NormalizeCode(string code)
    {
        // Remove dots and spaces, convert to uppercase
        return code.Replace(".", "").Replace(" ", "").ToUpperInvariant();
    }

    private static Dictionary<string, string> LoadCodesFromResource()
    {
        Assembly assembly = typeof(Icd10Repository).Assembly;
        string resourceName = "SSS.Quality1500.Data.Resources.icd10-codes.json";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
        {
            // Return empty dictionary if resource not found
            // In production, this should log a warning
            return new Dictionary<string, string>();
        }

        using StreamReader reader = new(stream);
        string json = reader.ReadToEnd();

        Icd10Data? data = JsonSerializer.Deserialize<Icd10Data>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data?.Codes ?? new Dictionary<string, string>();
    }

    private sealed class Icd10Data
    {
        public string Version { get; set; } = string.Empty;
        public string LastUpdated { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public Dictionary<string, string> Codes { get; set; } = [];
    }
}
