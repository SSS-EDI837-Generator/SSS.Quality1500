namespace SSS.Quality1500.Data.Repositories;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using System.Text.Json;

/// <summary>
/// Repository for ICD-10 code validation using external JSON file.
/// The file is located at Resources/icd10-codes.json relative to the application directory,
/// allowing updates without recompilation.
/// </summary>
public class Icd10Repository : IIcd10Repository
{
    private static readonly JsonSerializerOptions s_readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions s_writeOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _filePath;
    private readonly Dictionary<string, string> _codes;
    private readonly HashSet<string> _normalizedCodes;

    public Icd10Repository()
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icd10-codes.json");
        _codes = LoadCodesFromFile(_filePath);
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

        foreach (KeyValuePair<string, string> kvp in _codes)
        {
            if (NormalizeCode(kvp.Key).Equals(normalized, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        return null;
    }

    public bool AddCode(string code, string description)
    {
        string normalized = NormalizeCode(code);

        if (_normalizedCodes.Contains(normalized))
            return false;

        _codes[code] = description;
        _normalizedCodes.Add(normalized);
        return true;
    }

    public bool RemoveCode(string code)
    {
        string normalized = NormalizeCode(code);

        if (!_normalizedCodes.Contains(normalized))
            return false;

        // Find the actual key in the dictionary
        string? actualKey = _codes.Keys.FirstOrDefault(k =>
            NormalizeCode(k).Equals(normalized, StringComparison.OrdinalIgnoreCase));

        if (actualKey is null)
            return false;

        _codes.Remove(actualKey);
        _normalizedCodes.Remove(normalized);
        return true;
    }

    public List<Icd10CodeEntry> SearchCodes(string searchTerm, int maxResults = 100)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return _codes
                .Take(maxResults)
                .Select(kvp => new Icd10CodeEntry(kvp.Key, kvp.Value))
                .ToList();
        }

        string term = searchTerm.Trim();

        return _codes
            .Where(kvp =>
                kvp.Key.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                kvp.Value.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Take(maxResults)
            .Select(kvp => new Icd10CodeEntry(kvp.Key, kvp.Value))
            .ToList();
    }

    public Result<int, string> SaveChanges()
    {
        try
        {
            Icd10Data data = new()
            {
                Version = "1.0",
                LastUpdated = DateTime.Now.ToString("yyyy-MM-dd"),
                Source = "User-managed catalog",
                Codes = new Dictionary<string, string>(_codes)
            };

            string json = JsonSerializer.Serialize(data, s_writeOptions);
            File.WriteAllText(_filePath, json);

            return Result<int, string>.Ok(_codes.Count);
        }
        catch (Exception ex)
        {
            return Result<int, string>.Fail($"Error al guardar el archivo: {ex.Message}");
        }
    }

    private static string NormalizeCode(string code)
    {
        return code.Replace(".", "").Replace(" ", "").ToUpperInvariant();
    }

    private static Dictionary<string, string> LoadCodesFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            return new Dictionary<string, string>();

        string json = File.ReadAllText(filePath);
        Icd10Data? data = JsonSerializer.Deserialize<Icd10Data>(json, s_readOptions);

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
