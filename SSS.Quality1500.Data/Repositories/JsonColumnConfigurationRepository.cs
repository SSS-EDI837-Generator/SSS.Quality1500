namespace SSS.Quality1500.Data.Repositories;

using System.Text.Json;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Implementacion de IColumnConfigurationRepository usando archivo JSON.
/// Guarda la configuracion junto a la aplicacion.
/// </summary>
public sealed class JsonColumnConfigurationRepository : IColumnConfigurationRepository
{
    private const string FileName = "column-config.json";
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonColumnConfigurationRepository()
    {
        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(appDirectory, FileName);
    }

    /// <inheritdoc/>
    public Result<ColumnConfiguration, string> Load()
    {
        if (!File.Exists(_filePath))
            return Result<ColumnConfiguration, string>.Ok(ColumnConfiguration.Empty);

        try
        {
            string json = File.ReadAllText(_filePath);
            ColumnConfiguration? config = JsonSerializer.Deserialize<ColumnConfiguration>(json, JsonOptions);

            if (config is null)
                return Result<ColumnConfiguration, string>.Ok(ColumnConfiguration.Empty);

            return Result<ColumnConfiguration, string>.Ok(config);
        }
        catch (JsonException ex)
        {
            return Result<ColumnConfiguration, string>.Fail($"Error al leer configuracion: {ex.Message}");
        }
        catch (IOException ex)
        {
            return Result<ColumnConfiguration, string>.Fail($"Error de acceso al archivo: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public Result<bool, string> Save(ColumnConfiguration configuration)
    {
        try
        {
            ColumnConfiguration configToSave = new()
            {
                Version = configuration.Version,
                LastModified = DateTime.UtcNow,
                SelectedColumns = configuration.SelectedColumns
            };

            string json = JsonSerializer.Serialize(configToSave, JsonOptions);
            File.WriteAllText(_filePath, json);

            return Result<bool, string>.Ok(true);
        }
        catch (JsonException ex)
        {
            return Result<bool, string>.Fail($"Error al serializar configuracion: {ex.Message}");
        }
        catch (IOException ex)
        {
            return Result<bool, string>.Fail($"Error al escribir archivo: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            return Result<bool, string>.Fail($"Sin permisos para escribir: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public bool Exists() => File.Exists(_filePath);
}
