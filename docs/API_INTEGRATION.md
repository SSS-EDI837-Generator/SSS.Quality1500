# API_INTEGRATION.md

## Integración con API de Validación de NPI

Este documento especifica la integración con la API del cliente para validación de NPIs, siguiendo la Clean Architecture de SSS.Quality1500.

## Ubicación en la Arquitectura

```
Domain/
└── Interfaces/
    └── INpiApiClient.cs           # Contrato

Data/
├── ApiClients/
│   └── NpiApiClient.cs            # Implementación HTTP
├── Models/
│   ├── NpiValidationRequest.cs   # DTO de request
│   └── NpiValidationResponse.cs  # DTO de response
└── Extensions/
    └── ServiceCollectionExtensions.cs  # Registro en DI
```

## Contratos de Domain

### INpiApiClient (Domain/Interfaces/)

```csharp
namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

public interface INpiApiClient
{
    /// <summary>
    /// Valida un NPI contra la base de datos del cliente
    /// </summary>
    /// <param name="npi">NPI de 10 dígitos a validar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Result con información del provider o mensaje de error</returns>
    Task<Result<NpiValidationResponse, string>> ValidateNpiAsync(
        string npi,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Valida múltiples NPIs en una sola llamada (batch)
    /// </summary>
    /// <param name="npis">Lista de NPIs a validar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Result con lista de validaciones o mensaje de error</returns>
    Task<Result<List<NpiValidationResponse>, string>> ValidateBatchAsync(
        List<string> npis,
        CancellationToken cancellationToken = default);
}
```

## Modelos de Data

### NpiValidationRequest (Data/Models/)

```csharp
namespace SSS.Quality1500.Data.Models;

using System.Text.Json.Serialization;

public sealed class NpiValidationRequest
{
    [JsonPropertyName("npi")]
    public string Npi { get; init; } = string.Empty;
    
    [JsonPropertyName("clientId")]
    public string ClientId { get; init; } = string.Empty;
}
```

### NpiValidationResponse (Data/Models/)

```csharp
namespace SSS.Quality1500.Data.Models;

using System.Text.Json.Serialization;

public sealed class NpiValidationResponse
{
    [JsonPropertyName("count")]
    public int Count { get; init; }
    
    [JsonPropertyName("npi")]
    public string Npi { get; init; } = string.Empty;
    
    [JsonPropertyName("isValid")]
    public bool IsValid { get; init; }
    
    [JsonPropertyName("providerInfo")]
    public ProviderInfo? ProviderInfo { get; init; }
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
    
    [JsonPropertyName("validatedAt")]
    public DateTime ValidatedAt { get; init; }
}

public sealed class ProviderInfo
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
    
    [JsonPropertyName("taxonomy")]
    public string Taxonomy { get; init; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
}
```

## Implementación en Data Layer

### NpiApiClient (Data/ApiClients/)

```csharp
namespace SSS.Quality1500.Data.ApiClients;

using System.Net.Http.Json;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public sealed class NpiApiClient : INpiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NpiApiClient> _logger;
    private readonly string _clientId;
    
    public NpiApiClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<NpiApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Leer configuración
        IConfigurationSection apiConfig = configuration.GetSection("NpiApi");
        string baseUrl = apiConfig["BaseUrl"] ?? throw new InvalidOperationException("NpiApi:BaseUrl not configured");
        string apiKey = apiConfig["ApiKey"] ?? throw new InvalidOperationException("NpiApi:ApiKey not configured");
        _clientId = apiConfig["ClientId"] ?? throw new InvalidOperationException("NpiApi:ClientId not configured");
        
        // Configurar HttpClient
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        _httpClient.Timeout = TimeSpan.FromSeconds(apiConfig.GetValue<int>("Timeout", 30));
    }
    
    public async Task<Result<NpiValidationResponse, string>> ValidateNpiAsync(
        string npi,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating NPI {Npi}", npi);
            
            var request = new NpiValidationRequest
            {
                Npi = npi,
                ClientId = _clientId
            };
            
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "/api/validate/npi",
                request,
                cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "NPI validation failed with status {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                
                return Result<NpiValidationResponse, string>.Fail(
                    $"API returned {response.StatusCode}: {errorContent}");
            }
            
            NpiValidationResponse? result = await response.Content
                .ReadFromJsonAsync<NpiValidationResponse>(cancellationToken: cancellationToken);
            
            if (result == null)
            {
                return Result<NpiValidationResponse, string>.Fail("Empty response from API");
            }
            
            _logger.LogInformation(
                "NPI validation completed for {Npi}. IsValid: {IsValid}, Count: {Count}",
                npi, result.IsValid, result.Count);
            
            return Result<NpiValidationResponse, string>.Ok(result);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error validating NPI {Npi}", npi);
            return Result<NpiValidationResponse, string>.Fail(
                $"Network error: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning("NPI validation timeout for {Npi}", npi);
            return Result<NpiValidationResponse, string>.Fail(
                $"Request timeout: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating NPI {Npi}", npi);
            return Result<NpiValidationResponse, string>.Fail(
                $"Unexpected error: {ex.Message}");
        }
    }
    
    public async Task<Result<List<NpiValidationResponse>, string>> ValidateBatchAsync(
        List<string> npis,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating batch of {Count} NPIs", npis.Count);
            
            var request = new
            {
                npis = npis,
                clientId = _clientId
            };
            
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
                "/api/validate/npi/batch",
                request,
                cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result<List<NpiValidationResponse>, string>.Fail(
                    $"API returned {response.StatusCode}: {errorContent}");
            }
            
            var result = await response.Content
                .ReadFromJsonAsync<BatchValidationResponse>(cancellationToken: cancellationToken);
            
            if (result == null || result.Results == null)
            {
                return Result<List<NpiValidationResponse>, string>.Fail("Empty response from API");
            }
            
            _logger.LogInformation(
                "Batch validation completed. Processed: {Processed}",
                result.TotalProcessed);
            
            return Result<List<NpiValidationResponse>, string>.Ok(result.Results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in batch NPI validation");
            return Result<List<NpiValidationResponse>, string>.Fail(ex.Message);
        }
    }
}

internal sealed class BatchValidationResponse
{
    [JsonPropertyName("results")]
    public List<NpiValidationResponse> Results { get; init; } = new();
    
    [JsonPropertyName("totalProcessed")]
    public int TotalProcessed { get; init; }
    
    [JsonPropertyName("validatedAt")]
    public DateTime ValidatedAt { get; init; }
}
```

## Configuración

### appsettings.json

```json
{
  "NpiApi": {
    "BaseUrl": "https://api.client.com/v1",
    "ApiKey": "",
    "ClientId": "",
    "Timeout": 30
  }
}
```

### appsettings.Development.json

```json
{
  "NpiApi": {
    "BaseUrl": "https://api-dev.client.com/v1",
    "ApiKey": "dev-api-key-here",
    "ClientId": "dev-client-id",
    "Timeout": 60
  }
}
```

### appsettings.Production.json

```json
{
  "NpiApi": {
    "BaseUrl": "https://api.client.com/v1",
    "ApiKey": "prod-api-key-here",
    "ClientId": "prod-client-id",
    "Timeout": 30
  }
}
```

## Retry Policy con Polly

### Instalación

```bash
dotnet add package Microsoft.Extensions.Http.Polly
```

### Configuración en ServiceCollectionExtensions

```csharp
// Data/Extensions/ServiceCollectionExtensions.cs
namespace SSS.Quality1500.Data.Extensions;

using Polly;
using Polly.Extensions.Http;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Data.ApiClients;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar HttpClient con Polly retry policy
        services.AddHttpClient<INpiApiClient, NpiApiClient>()
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
        
        return services;
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    // Log retry attempt
                    var logger = context.GetLogger();
                    logger?.LogWarning(
                        "Retry {RetryCount} after {Delay}s due to {Reason}",
                        retryCount, timespan.TotalSeconds, outcome.Result?.StatusCode);
                });
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) =>
                {
                    // Log circuit break
                },
                onReset: () =>
                {
                    // Log circuit reset
                });
    }
}
```

## Caching de Resultados

### CachedNpiApiClient (Data/ApiClients/)

```csharp
namespace SSS.Quality1500.Data.ApiClients;

using Microsoft.Extensions.Caching.Memory;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Data.Models;

public sealed class CachedNpiApiClient : INpiApiClient
{
    private readonly INpiApiClient _innerClient;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(24);
    
    public CachedNpiApiClient(NpiApiClient innerClient, IMemoryCache cache)
    {
        _innerClient = innerClient;
        _cache = cache;
    }
    
    public async Task<Result<NpiValidationResponse, string>> ValidateNpiAsync(
        string npi,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"npi_validation_{npi}";
        
        if (_cache.TryGetValue(cacheKey, out Result<NpiValidationResponse, string>? cached) 
            && cached != null)
        {
            return cached;
        }
        
        Result<NpiValidationResponse, string> result = 
            await _innerClient.ValidateNpiAsync(npi, cancellationToken);
        
        // Solo cachear resultados exitosos
        if (result.IsSuccess)
        {
            _cache.Set(cacheKey, result, _cacheExpiration);
        }
        
        return result;
    }
    
    public Task<Result<List<NpiValidationResponse>, string>> ValidateBatchAsync(
        List<string> npis,
        CancellationToken cancellationToken = default)
    {
        // Batch no se cachea - delegar al cliente interno
        return _innerClient.ValidateBatchAsync(npis, cancellationToken);
    }
}
```

### Registro con Caching

```csharp
// Data/Extensions/ServiceCollectionExtensions.cs
public static IServiceCollection AddDataServices(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // Registrar memory cache
    services.AddMemoryCache();
    
    // Registrar cliente base
    services.AddHttpClient<NpiApiClient>()
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy());
    
    // Registrar cliente con cache como implementación de la interfaz
    services.AddScoped<INpiApiClient, CachedNpiApiClient>();
    
    return services;
}
```

## Rate Limiting

### Implementación con SemaphoreSlim

```csharp
// Data/ApiClients/RateLimitedNpiApiClient.cs
namespace SSS.Quality1500.Data.ApiClients;

public sealed class RateLimitedNpiApiClient : INpiApiClient
{
    private readonly INpiApiClient _innerClient;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<RateLimitedNpiApiClient> _logger;
    
    public RateLimitedNpiApiClient(
        NpiApiClient innerClient,
        ILogger<RateLimitedNpiApiClient> logger,
        int maxConcurrentRequests = 10)
    {
        _innerClient = innerClient;
        _logger = logger;
        _semaphore = new SemaphoreSlim(maxConcurrentRequests);
    }
    
    public async Task<Result<NpiValidationResponse, string>> ValidateNpiAsync(
        string npi,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            return await _innerClient.ValidateNpiAsync(npi, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public async Task<Result<List<NpiValidationResponse>, string>> ValidateBatchAsync(
        List<string> npis,
        CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        
        try
        {
            return await _innerClient.ValidateBatchAsync(npis, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

## Testing

### Unit Test con Mock

```csharp
// Data.Tests/ApiClients/NpiApiClientTests.cs
namespace SSS.Quality1500.Data.Tests.ApiClients;

using System.Net;
using System.Net.Http;
using Moq;
using Moq.Protected;
using Xunit;

public class NpiApiClientTests
{
    [Fact]
    public async Task ValidateNpiAsync_WithValidNpi_ReturnsSuccess()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{
                    ""count"": 1,
                    ""npi"": ""1234567890"",
                    ""isValid"": true,
                    ""validatedAt"": ""2026-01-20T00:00:00Z""
                }")
            });
        
        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://api.test.com")
        };
        
        var config = new Mock<IConfiguration>();
        config.Setup(c => c.GetSection("NpiApi")["BaseUrl"]).Returns("https://api.test.com");
        config.Setup(c => c.GetSection("NpiApi")["ApiKey"]).Returns("test-key");
        config.Setup(c => c.GetSection("NpiApi")["ClientId"]).Returns("test-client");
        
        var logger = new Mock<ILogger<NpiApiClient>>();
        
        var client = new NpiApiClient(httpClient, config.Object, logger.Object);
        
        // Act
        Result<NpiValidationResponse, string> result = 
            await client.ValidateNpiAsync("1234567890");
        
        // Assert
        Assert.True(result.IsSuccess);
        result.OnSuccess(response =>
        {
            Assert.Equal(1, response.Count);
            Assert.True(response.IsValid);
        });
    }
}
```

## Endpoints de API

### POST /api/validate/npi

**Request:**
```json
{
  "npi": "1234567890",
  "clientId": "CLIENT_ID"
}
```

**Response (200 OK) - NPI Found:**
```json
{
  "count": 1,
  "npi": "1234567890",
  "isValid": true,
  "providerInfo": {
    "name": "Dr. John Smith",
    "type": "Individual",
    "taxonomy": "207Q00000X",
    "status": "Active"
  },
  "validatedAt": "2026-01-20T14:30:00Z"
}
```

**Response (200 OK) - NPI Not Found:**
```json
{
  "count": 0,
  "npi": "1234567890",
  "isValid": false,
  "message": "NPI not found in client database",
  "validatedAt": "2026-01-20T14:30:00Z"
}
```

**Response (429 Too Many Requests):**
```json
{
  "error": "RateLimitExceeded",
  "message": "Too many requests",
  "retryAfter": 60
}
```
