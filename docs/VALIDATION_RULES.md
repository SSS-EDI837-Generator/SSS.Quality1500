# VALIDATION_RULES.md

## Reglas de Validación - Adaptadas a Clean Architecture

Este documento especifica las reglas de validación para SSS.Quality1500, siguiendo la arquitectura definida en CLAUDE.md.

## Ubicación en la Arquitectura

### Domain Layer (Definiciones)
```
Domain/
├── Models/
│   └── ValidationError.cs
├── Enums/
│   └── ErrorSeverity.cs
└── Interfaces/
    ├── IValidator.cs
    └── IValidationRule.cs
```

### Business Layer (Implementaciones)
```
Business/
├── Validators/
│   ├── NpiValidator.cs
│   ├── DateValidator.cs
│   ├── CptValidator.cs
│   └── IcdValidator.cs
└── Services/
    └── ValidationEngine.cs
```

## Contratos de Domain

### IValidationRule (Domain/Interfaces/)
```csharp
namespace SSS.Quality1500.Domain.Interfaces;

public interface IValidationRule<in TEntity>
{
    string RuleName { get; }
    string RuleCode { get; }
    ErrorSeverity Severity { get; }
    
    Task<Result<bool, ValidationError>> ValidateAsync(TEntity entity);
}
```

### IValidator (Domain/Interfaces/)
```csharp
namespace SSS.Quality1500.Domain.Interfaces;

public interface IValidator<in TEntity>
{
    Task<Result<List<ValidationError>, string>> ValidateAsync(TEntity entity);
}
```

### ValidationError (Domain/Models/)
```csharp
namespace SSS.Quality1500.Domain.Models;

public sealed class ValidationError
{
    public string RuleCode { get; init; } = string.Empty;
    public string RuleName { get; init; } = string.Empty;
    public string FieldName { get; init; } = string.Empty;
    public ErrorSeverity Severity { get; init; }
    public string Message { get; init; } = string.Empty;
    public string CurrentValue { get; init; } = string.Empty;
    public string ExpectedValue { get; init; } = string.Empty;
    public DateTime ValidatedAt { get; init; } = DateTime.UtcNow;
}
```

## Categorías de Validación

### 1. Validación de NPIs

#### NPI-001: Formato de NPI
**Severidad:** Error  
**Campos:** RenderingNpi, ReferringNpi, BillingNpi  
**Regla:** NPI debe ser numérico de exactamente 10 dígitos

**Implementación:**
```csharp
// Business/Validators/NpiFormatValidator.cs
namespace SSS.Quality1500.Business.Validators;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Domain.Enums;

public sealed class NpiFormatValidator : IValidationRule<string>
{
    public string RuleName => "NPI Format Validation";
    public string RuleCode => "NPI-001";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public Task<Result<bool, ValidationError>> ValidateAsync(string npi)
    {
        if (string.IsNullOrWhiteSpace(npi))
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "NPI",
                Severity = Severity,
                Message = "NPI no puede estar vacío",
                CurrentValue = npi ?? string.Empty,
                ExpectedValue = "10 dígitos numéricos"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        if (npi.Length != 10 || !npi.All(char.IsDigit))
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "NPI",
                Severity = Severity,
                Message = "NPI debe contener exactamente 10 dígitos numéricos",
                CurrentValue = npi,
                ExpectedValue = "10 dígitos numéricos"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
}
```

#### NPI-002: Existencia de NPI en Base de Datos del Cliente
**Severidad:** Error  
**Campos:** RenderingNpi, ReferringNpi, BillingNpi  
**Regla:** NPI debe existir en la base de datos del cliente (validación vía API)

**Implementación:**
```csharp
// Business/Validators/NpiExistenceValidator.cs
namespace SSS.Quality1500.Business.Validators;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;
using SSS.Quality1500.Domain.Enums;

public sealed class NpiExistenceValidator : IValidationRule<string>
{
    private readonly INpiApiClient _apiClient;
    
    public string RuleName => "NPI Existence Validation";
    public string RuleCode => "NPI-002";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public NpiExistenceValidator(INpiApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    
    public async Task<Result<bool, ValidationError>> ValidateAsync(string npi)
    {
        // Primero validar formato
        var formatValidator = new NpiFormatValidator();
        Result<bool, ValidationError> formatResult = await formatValidator.ValidateAsync(npi);
        
        if (!formatResult.IsSuccess)
            return formatResult;
        
        // Validar existencia via API
        Result<NpiValidationResponse, string> apiResult = await _apiClient.ValidateNpiAsync(npi);
        
        return apiResult.Match(
            success: response =>
            {
                if (response.Count == 0)
                {
                    var error = new ValidationError
                    {
                        RuleCode = RuleCode,
                        RuleName = RuleName,
                        FieldName = "NPI",
                        Severity = Severity,
                        Message = $"NPI {npi} no encontrado en base de datos del cliente",
                        CurrentValue = npi,
                        ExpectedValue = "NPI existente en base de datos"
                    };
                    return Result<bool, ValidationError>.Fail(error);
                }
                
                return Result<bool, ValidationError>.Ok(true);
            },
            failure: errorMessage =>
            {
                var error = new ValidationError
                {
                    RuleCode = RuleCode,
                    RuleName = RuleName,
                    FieldName = "NPI",
                    Severity = ErrorSeverity.Critical,
                    Message = $"Error al validar NPI: {errorMessage}",
                    CurrentValue = npi,
                    ExpectedValue = "Validación exitosa"
                };
                return Result<bool, ValidationError>.Fail(error);
            }
        );
    }
}
```

#### NPI-003: Checksum de NPI (Algoritmo Luhn)
**Severidad:** Error  
**Campos:** RenderingNpi, ReferringNpi, BillingNpi  
**Regla:** NPI debe pasar validación de checksum usando algoritmo Luhn

**Implementación:**
```csharp
// Business/Validators/NpiChecksumValidator.cs
namespace SSS.Quality1500.Business.Validators;

public sealed class NpiChecksumValidator : IValidationRule<string>
{
    public string RuleName => "NPI Checksum Validation";
    public string RuleCode => "NPI-003";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public Task<Result<bool, ValidationError>> ValidateAsync(string npi)
    {
        // Primero validar formato
        var formatValidator = new NpiFormatValidator();
        Result<bool, ValidationError> formatResult = formatValidator.ValidateAsync(npi).Result;
        
        if (!formatResult.IsSuccess)
            return Task.FromResult(formatResult);
        
        // Agregar prefijo "80840" al NPI para validación Luhn
        string npiWithPrefix = "80840" + npi;
        
        if (!IsValidLuhnChecksum(npiWithPrefix))
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "NPI",
                Severity = Severity,
                Message = $"NPI {npi} tiene checksum inválido",
                CurrentValue = npi,
                ExpectedValue = "NPI con checksum válido (algoritmo Luhn)"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
    
    private static bool IsValidLuhnChecksum(string number)
    {
        int sum = 0;
        bool alternate = false;
        
        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = number[i] - '0';
            
            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }
            
            sum += digit;
            alternate = !alternate;
        }
        
        return sum % 10 == 0;
    }
}
```

### 2. Validación de Fechas

#### DATE-001: Formato de Fecha de Servicio
**Severidad:** Error  
**Campos:** DateFrom, DateTo

```csharp
// Business/Validators/DateFormatValidator.cs
namespace SSS.Quality1500.Business.Validators;

public sealed class DateFormatValidator : IValidationRule<DateTime>
{
    public string RuleName => "Date Format Validation";
    public string RuleCode => "DATE-001";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public Task<Result<bool, ValidationError>> ValidateAsync(DateTime date)
    {
        if (date == default(DateTime))
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "ServiceDate",
                Severity = Severity,
                Message = "Fecha de servicio es inválida o faltante",
                CurrentValue = date.ToString("MM/dd/yyyy"),
                ExpectedValue = "Fecha válida"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
}
```

#### DATE-002: Fecha No Puede Ser Futura
**Severidad:** Error  
**Campos:** DateFrom, DateTo

```csharp
// Business/Validators/DateNotFutureValidator.cs
namespace SSS.Quality1500.Business.Validators;

public sealed class DateNotFutureValidator : IValidationRule<DateTime>
{
    public string RuleName => "Date Not Future Validation";
    public string RuleCode => "DATE-002";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public Task<Result<bool, ValidationError>> ValidateAsync(DateTime date)
    {
        if (date > DateTime.Today)
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "ServiceDate",
                Severity = Severity,
                Message = "Fecha de servicio no puede ser en el futuro",
                CurrentValue = date.ToString("MM/dd/yyyy"),
                ExpectedValue = $"Fecha <= {DateTime.Today:MM/dd/yyyy}"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
}
```

#### DATE-003: Rango de Fechas Válido
**Severidad:** Error  
**Campos:** DateFrom, DateTo

```csharp
// Business/Validators/DateRangeValidator.cs
namespace SSS.Quality1500.Business.Validators;

public sealed class DateRangeValidator : IValidationRule<(DateTime From, DateTime To)>
{
    public string RuleName => "Date Range Validation";
    public string RuleCode => "DATE-003";
    public ErrorSeverity Severity => ErrorSeverity.Error;
    
    public Task<Result<bool, ValidationError>> ValidateAsync((DateTime From, DateTime To) dates)
    {
        if (dates.To < dates.From)
        {
            var error = new ValidationError
            {
                RuleCode = RuleCode,
                RuleName = RuleName,
                FieldName = "ServiceDateRange",
                Severity = Severity,
                Message = "Fecha de fin no puede ser anterior a fecha de inicio",
                CurrentValue = $"{dates.From:MM/dd/yyyy} - {dates.To:MM/dd/yyyy}",
                ExpectedValue = "DateTo >= DateFrom"
            };
            return Task.FromResult(Result<bool, ValidationError>.Fail(error));
        }
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
}
```

### 3. Motor de Validación

#### ValidationEngine (Business/Services/)
```csharp
namespace SSS.Quality1500.Business.Services;

using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

public sealed class ValidationEngine : IValidator<ClaimRecord>
{
    private readonly IEnumerable<IValidationRule<ClaimRecord>> _rules;
    private readonly ILogger<ValidationEngine> _logger;
    
    public ValidationEngine(
        IEnumerable<IValidationRule<ClaimRecord>> rules,
        ILogger<ValidationEngine> logger)
    {
        _rules = rules;
        _logger = logger;
    }
    
    public async Task<Result<List<ValidationError>, string>> ValidateAsync(ClaimRecord claim)
    {
        var errors = new List<ValidationError>();
        
        foreach (var rule in _rules)
        {
            try
            {
                Result<bool, ValidationError> result = await rule.ValidateAsync(claim);
                
                result.OnFailure(error => errors.Add(error));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Error executing validation rule {RuleName} for claim {ClaimId}",
                    rule.RuleName, claim.ClaimId);
                
                errors.Add(new ValidationError
                {
                    RuleCode = rule.RuleCode,
                    RuleName = rule.RuleName,
                    Severity = ErrorSeverity.Critical,
                    Message = $"Error interno en validación: {ex.Message}"
                });
            }
        }
        
        return Result<List<ValidationError>, string>.Ok(errors);
    }
}
```

## Registro de Validadores en DI

```csharp
// Business/Extensions/ServiceCollectionExtensions.cs
namespace SSS.Quality1500.Business.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Validadores individuales
        services.AddScoped<IValidationRule<string>, NpiFormatValidator>();
        services.AddScoped<IValidationRule<string>, NpiExistenceValidator>();
        services.AddScoped<IValidationRule<string>, NpiChecksumValidator>();
        services.AddScoped<IValidationRule<DateTime>, DateFormatValidator>();
        services.AddScoped<IValidationRule<DateTime>, DateNotFutureValidator>();
        
        // Motor de validación
        services.AddScoped<IValidator<ClaimRecord>, ValidationEngine>();
        
        return services;
    }
}
```

## Prioridad de Implementación

### Fase 1 (Esta Semana)
- ✅ NPI-001: Formato de NPI
- ✅ NPI-002: Existencia en BD via API
- ✅ DATE-001: Formato de fecha
- ✅ DATE-002: Fecha no futura
- ✅ DATE-003: Rango de fechas válido

### Fase 2 (Próxima Semana)
- ⏳ NPI-003: Checksum Luhn
- ⏳ CPT-001: Formato de código CPT
- ⏳ ICD-001: Formato de código ICD-10
- ⏳ AMT-001: Validación de montos

### Fase 3 (Futuro)
- ⏳ POS-001: Código de Place of Service válido
- ⏳ ICD-002: Diagnosis Pointer válido
- ⏳ NPI-004: Lista de exclusión OIG

## Configuración de Validaciones

Las validaciones deben ser configurables en `appsettings.json`:

```json
{
  "ValidationRules": {
    "NPI": {
      "EnableFormatValidation": true,
      "EnableExistenceValidation": true,
      "EnableChecksumValidation": true,
      "ApiEndpoint": "https://api.client.com/v1/validate/npi"
    },
    "Dates": {
      "EnableFutureCheck": true,
      "EnableRangeCheck": true,
      "TimelyFilingDays": 365
    }
  }
}
```

## Testing de Validadores

```csharp
// Business.Tests/Validators/NpiFormatValidatorTests.cs
namespace SSS.Quality1500.Business.Tests.Validators;

public class NpiFormatValidatorTests
{
    [Fact]
    public async Task ValidateAsync_WithValidNpi_ReturnsSuccess()
    {
        // Arrange
        var validator = new NpiFormatValidator();
        string validNpi = "1234567890";
        
        // Act
        Result<bool, ValidationError> result = await validator.ValidateAsync(validNpi);
        
        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task ValidateAsync_WithInvalidLength_ReturnsFailure()
    {
        // Arrange
        var validator = new NpiFormatValidator();
        string invalidNpi = "123"; // Solo 3 dígitos
        
        // Act
        Result<bool, ValidationError> result = await validator.ValidateAsync(invalidNpi);
        
        // Assert
        Assert.False(result.IsSuccess);
        result.OnFailure(error =>
        {
            Assert.Equal("NPI-001", error.RuleCode);
            Assert.Contains("10 dígitos", error.Message);
        });
    }
}
```
