namespace SSS.Quality1500.Data.Repositories;

using SSS.Quality1500.Domain.Interfaces;
using System.Text.Json;

/// <summary>
/// Repository for ICD-10 code validation using external JSON file.
/// The file is located at Resources/icd10-codes.json relative to the application directory,
/// allowing updates without recompilation.
/// </summary>
public class Icd10Repository : IIcd10Repository
{
    private static readonly JsonSerializerOptions s_jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Dictionary<string, string> _codes;
    private readonly HashSet<string> _normalizedCodes;

    public Icd10Repository()
    {
        _codes = LoadCodesFromFile();
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

    private static Dictionary<string, string> LoadCodesFromFile()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icd10-codes.json");

        if (!File.Exists(filePath))
            return new Dictionary<string, string>();

        string json = File.ReadAllText(filePath);
        Icd10Data? data = JsonSerializer.Deserialize<Icd10Data>(json, s_jsonOptions);

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
