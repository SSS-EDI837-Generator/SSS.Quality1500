# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview
**SSS.Quality1500** is a WPF desktop application for healthcare claims quality review (CMS-1500 forms). Built with .NET 10 using Clean Architecture and strict MVVM.

## Commands

```bash
# Restore dependencies
dotnet restore

# Build (Debug)
dotnet build

# Build (Release)
dotnet build -c Release

# Run the application
dotnet run --project SSS.Quality1500.Presentation

# Clean build artifacts
dotnet clean

# Verify build with zero warnings (pre-commit check)
dotnet build --nologo -v quiet
```

## Architecture

### Dependency Flow (innermost to outermost)
```
Common ← Domain ← Data ← Business ← Presentation
```

### Layer Responsibilities
- **Common**: Transversal utilities only (environment detection, version info, lazy services). No business logic, NO dependency on other layers.
- **Domain**: Entities, Value Objects, `Result<T,E>`, and interfaces (contracts). No framework dependencies.
- **Data**: Implements Domain contracts (DBF readers via NDbfReader, SQLite via EF Core).
- **Business**: Orchestrates use cases using Domain entities and Data implementations. Contains LoggerInitializer.
- **Presentation**: WPF + MVVM with CommunityToolkit.Mvvm and MaterialDesignThemes.

Each layer has a `LAYER.md` with detailed documentation.

## Key Patterns

### Result Pattern (`Domain/Models/Result.cs`)
Use `Result<TSuccess, TFailure>` instead of exceptions for expected failures:
```csharp
Result<DataTable, string> result = await service.GetDataAsync();
result.OnSuccess(data => Process(data))
      .OnFailure(error => Log(error));
```

### Dependency Injection
Register services in each layer's `Extensions/ServiceCollectionExtensions.cs`:
- `AddCommonServices()` → `AddDataServices(config)` → `AddBusinessServices(config)`

Contracts are defined in **Domain**, implementations in **Data**:
```csharp
// Domain: IDbfReader (contract)
// Data: DbfReader (implementation)
services.AddTransient<IDbfReader, DbfReader>();
```

### MVVM Rules
- **No code-behind** (except `InitializeComponent`)
- Use `[ObservableProperty]` and `[RelayCommand]` from CommunityToolkit.Mvvm
- Dialogs via `IDialogService`, navigation via services

### Aggregates (Domain-Driven Design)

**Two types of Aggregates in this project:**

#### 1. Domain Aggregates (DDD Pattern)
- **Location**: `Domain/Aggregates/` folder
- **Contracts**: `Domain/Aggregates/Abstractions/`
- **Purpose**: Group related entities with consistency rules, single root entity
- **When to use**: When entities must maintain transactional consistency together

#### 2. Aggregate Services (Dependency Management)
- **Location**: `Business/Aggregates/` folder  
- **Contracts**: `Business/Aggregates/Abstractions/`
- **Purpose**: Compose multiple services to reduce constructor dependencies (helps meet 3-parameter limit)
- **When to use**: When a service needs 4+ dependencies

**Example of Aggregate Service:**
```csharp
// Business/Aggregates/Abstractions/IDataAggregate.cs
public interface IDataAggregate
{
    IDbfReader DbfReader { get; }
    IRepository Repository { get; }
    IValidator Validator { get; }
}

// Business/Aggregates/DataAggregate.cs
public class DataAggregate(IDbfReader dbfReader, 
                          IRepository repository, 
                          IValidator validator) : IDataAggregate
{
    public IDbfReader DbfReader { get; } = dbfReader;
    public IRepository Repository { get; } = repository;
    public IValidator Validator { get; } = validator;
}

// Usage in a service - reduces from 4 parameters to 2
public class ClaimService(IDataAggregate data, ILogger logger)
{
    public async Task ProcessAsync()
    {
        var result = await data.DbfReader.GetAllAsDataTableAsync("file.dbf");
        // ...
    }
}
```

**Folder Structure:**
```
Domain/
  Aggregates/              # Domain aggregates (DDD)
    Abstractions/          # Contracts for domain aggregates
Business/
  Aggregates/              # Aggregate services (DI composition)
    Abstractions/          # Contracts for aggregate services
```

## Tech Stack
- **.NET 10** (net10.0-windows for Presentation)
- **WPF** with MaterialDesignThemes 5.x
- **CommunityToolkit.Mvvm** for MVVM infrastructure
- **NDbfReader** for DBF file access
- **EF Core + SQLite** for persistence
- **Serilog** for logging

## Code Conventions

### General Rules
- File-scoped namespaces (always)
- Explicit types for simple types (not `var`)
  ```csharp
  // ✅ CORRECT
  string recordCFile = paths.recordCPath;
  
  // ❌ AVOID
  var recordCFile = paths.recordCPath;
  ```

### Primary Constructors (C# 12+)
**Rule**: Always use primary constructors when possible.

**What are Primary Constructors?**
A C# 12+ feature that allows declaring constructor parameters directly in the class declaration, eliminating boilerplate code.

**Benefits:**
- Less boilerplate code
- Cleaner, more readable class definitions
- Parameters automatically available throughout the class
- Works seamlessly with dependency injection

**Examples:**
```csharp
// ❌ OLD WAY - Traditional constructor (avoid)
public class ClaimService
{
    private readonly IDbfReader _dbfReader;
    private readonly ILogger _logger;
    
    public ClaimService(IDbfReader dbfReader, ILogger logger)
    {
        _dbfReader = dbfReader;
        _logger = logger;
    }
    
    public async Task ProcessAsync()
    {
        await _dbfReader.GetAllAsDataTableAsync("file.dbf");
    }
}

// ✅ CORRECT - Primary constructor (preferred)
public class ClaimService(IDbfReader dbfReader, ILogger logger)
{
    public async Task ProcessAsync()
    {
        await dbfReader.GetAllAsDataTableAsync("file.dbf");
    }
}

// ✅ CORRECT - With property initialization
public class DataAggregate(IDbfReader dbfReader, 
                          IRepository repository) : IDataAggregate
{
    public IDbfReader DbfReader { get; } = dbfReader;
    public IRepository Repository { get; } = repository;
}
```

**When to use traditional constructors:**
- When you need complex initialization logic
- When parameters need validation before assignment
- When working with base class constructors that require complex setup

### File Size Limits (STRICT)
- **Maximum 500 lines** per `.cs` file (excluding blank lines)
- **Exceptions**: Autogenerated files (`*.g.cs`, `*Designer.cs`) and `bin/`, `obj/`, `.git/` folders
- **Enforcement**: Pre-commit hooks and CI checks
- **Rationale**: Promotes Single Responsibility Principle (SRP), improves maintainability, and faster code reviews
- **If a file exceeds 500 lines**:
  - Split into partial classes
  - Extract helper services or utilities
  - Refactor to follow SRP more strictly

### Constructor and Method Parameter Limits (STRICT)
- **Target**: Maximum **3 parameters**
- **Absolute maximum**: **5 parameters** (never exceed)
- **Applies to**: All constructors, methods, and functions

**Why this matters:**
- Reduces cognitive load
- Makes testing easier
- Encourages proper use of objects and abstractions

**Solutions when you need more parameters:**
```csharp
// ❌ BAD - Too many parameters
public class Service(IRepo1 repo1, IRepo2 repo2, IRepo3 repo3, 
                     IRepo4 repo4, IRepo5 repo5, IRepo6 repo6) { }

// ✅ GOOD - Use aggregate service
public interface IDataAggregate
{
    IRepo1 Repo1 { get; }
    IRepo2 Repo2 { get; }
    IRepo3 Repo3 { get; }
}

public class Service(IDataAggregate aggregate, ILogger logger) { }

// ✅ GOOD - Use configuration object
public record ServiceConfig(string Path, int Timeout, bool EnableCache);
public class Service(ServiceConfig config, ILogger logger) { }
```

## Critical Architectural Rules

### Dependency Matrix
| Layer | Can Use | CANNOT Use |
|-------|---------|------------|
| **Common** | NuGet packages only | Domain, Data, Business, Presentation |
| **Domain** | Nothing (pure core) | Common, Data, Business, Presentation |
| **Data** | Domain, Common | Business, Presentation |
| **Business** | Domain, Data, Common | Presentation |
| **Presentation** | Domain, Business, Common | Data (use Business instead) |

### Critical Violations to Avoid

**❌ NEVER:**
```csharp
// Domain depending on Common
namespace SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Common.Something; // VIOLATION

// Data depending on Business
namespace SSS.Quality1500.Data.Services;
using SSS.Quality1500.Business.Models; // VIOLATION

// Presentation depending on Data directly
namespace SSS.Quality1500.Presentation.ViewModels;
using SSS.Quality1500.Data.Services; // VIOLATION
```

**✅ CORRECT:**
```csharp
// Domain defines contract
namespace SSS.Quality1500.Domain.Interfaces;
public interface IDataService { }

// Data implements Domain contract
namespace SSS.Quality1500.Data.Services;
using SSS.Quality1500.Domain.Interfaces;
public class DataService : IDataService { }

// Business uses both
namespace SSS.Quality1500.Business.Services;
using SSS.Quality1500.Domain.Interfaces;
```

### Pre-Commit Verification
Before committing, verify:
- [ ] `.csproj` files only reference allowed layers
- [ ] `using` statements follow dependency rules
- [ ] Namespaces match physical folder structure
- [ ] `dotnet build` succeeds with 0 warnings

## Healthcare Domain Documentation
For healthcare-specific context (CMS-1500, NPI validation, etc.), consult:
- `docs/HEALTHCARE_CONTEXT.md` - CMS-1500 form context
- `docs/VALIDATION_RULES.md` - Business validation rules
- `docs/API_INTEGRATION.md` - NPI API integration patterns
- `docs/UI_PATTERNS.md` - UI patterns for claim review

## Common Healthcare Validation Patterns

### NPI Validation
```csharp
// Business/Validators/NpiValidator.cs
public class NpiValidator(INpiApiClient apiClient) : IValidationRule<string>
{
    public async Task<Result<bool, ValidationError>> ValidateAsync(string npi)
    {
        // Format check
        if (npi.Length != 10 || !npi.All(char.IsDigit))
            return Result<bool, ValidationError>.Fail(new ValidationError(...));
        
        // API check
        Result<NpiValidationResponse, string> result = 
            await apiClient.ValidateNpiAsync(npi);
        
        return result.Match(
            success: r => r.Count > 0 
                ? Result<bool, ValidationError>.Ok(true)
                : Result<bool, ValidationError>.Fail(new ValidationError(...)),
            failure: e => Result<bool, ValidationError>.Fail(new ValidationError(...))
        );
    }
}
```

### Date Validation
```csharp
// Business/Validators/DateValidator.cs
public class DateRangeValidator : IValidationRule<(DateTime From, DateTime To)>
{
    public Task<Result<bool, ValidationError>> ValidateAsync((DateTime From, DateTime To) dates)
    {
        if (dates.To < dates.From)
            return Task.FromResult(Result<bool, ValidationError>.Fail(...));
        
        return Task.FromResult(Result<bool, ValidationError>.Ok(true));
    }
}
```

Consult `docs/VALIDATION_RULES.md` for complete validation specifications.

## Common UI Patterns

### Error Review with Image Zoom
```csharp
// Presentation/ViewModels/ErrorReviewViewModel.cs
public partial class ErrorReviewViewModel(
    IImageService imageService,
    IValidationService validationService) : ObservableObject
{
    [ObservableProperty]
    private ClaimRecord? _currentClaim;
    
    [ObservableProperty]
    private BitmapImage? _formImage;
    
    [RelayCommand]
    private async Task LoadClaimAsync(string claimId)
    {
        Result<ClaimRecord, string> result = 
            await validationService.GetClaimByIdAsync(claimId);
        
        result.OnSuccess(claim => CurrentClaim = claim);
    }
    
    [RelayCommand]
    private void ZoomToField(string fieldName)
    {
        Rect fieldArea = imageService.GetFieldCoordinates(fieldName);
        FocusArea = fieldArea;
        ZoomLevel = CalculateZoomLevel(fieldArea);
    }
}
```

Consult `docs/UI_PATTERNS.md` for complete UI implementation patterns.

## Testing Patterns

### Unit Testing Validators
```csharp
// Business.Tests/Validators/NpiValidatorTests.cs
public class NpiValidatorTests
{
    [Fact]
    public async Task ValidateAsync_WithValidNpi_ReturnsSuccess()
    {
        // Arrange
        var mockApiClient = new Mock<INpiApiClient>();
        mockApiClient
            .Setup(x => x.ValidateNpiAsync(It.IsAny<string>(), default))
            .ReturnsAsync(Result<NpiValidationResponse, string>.Ok(
                new NpiValidationResponse { Count = 1, IsValid = true }));
        
        var validator = new NpiExistenceValidator(mockApiClient.Object);
        
        // Act
        Result<bool, ValidationError> result = 
            await validator.ValidateAsync("1234567890");
        
        // Assert
        Assert.True(result.IsSuccess);
    }
}
```

### Integration Testing with API
```csharp
// Data.Tests/ApiClients/NpiApiClientTests.cs
[Trait("Category", "Integration")]
public class NpiApiClientIntegrationTests
{
    [Fact]
    public async Task ValidateNpiAsync_WithRealApi_ReturnsValidResponse()
    {
        // Arrange - requires test API configuration
        var client = CreateConfiguredClient();
        
        // Act
        Result<NpiValidationResponse, string> result = 
            await client.ValidateNpiAsync("1234567890");
        
        // Assert
        Assert.True(result.IsSuccess);
    }
}
```



## Git Workflow

### Feature Development
```bash
# Create feature branch from main
git checkout main
git pull origin main
git checkout -b feature/nueva-funcionalidad

# After implementation and testing
git add .
git commit -m "feat: descripción de la funcionalidad"
git push origin feature/nueva-funcionalidad

# Create PR (requires gh CLI)
gh pr create --base main --head feature/nueva-funcionalidad \
  --title "Nueva Funcionalidad" --body "Descripción del cambio"
```

### Release Process
```bash
# Create release branch
git checkout main
git pull origin main
git checkout -b release/vX.Y.Z

# Update version in .csproj files, commit, and push
git add .
git commit -m "chore: bump version to vX.Y.Z"
git push origin release/vX.Y.Z

# After testing, create tag and merge
git tag -a vX.Y.Z -m "Release vX.Y.Z - Description"
git push origin vX.Y.Z

# Merge to main with merge commit (preserves history)
git checkout main
git merge release/vX.Y.Z --no-ff -m "Merge release/vX.Y.Z into main"
git push origin main

# Create GitHub release
gh release create vX.Y.Z --title "Release vX.Y.Z" \
  --notes "Descripción detallada del release"
```

### Comparing Tags with Main
```bash
# Fetch latest
git fetch --all --tags

# See commits in main not in tag
git --no-pager log TAG_NAME..origin/main --oneline --graph

# See file differences
git diff --stat TAG_NAME..origin/main

# See specific file diff
git --no-pager diff TAG_NAME..origin/main -- path/to/file
```

## Commit Message Standards
Follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat`: New feature
- `fix`: Bug fix
- `chore`: Maintenance tasks
- `docs`: Documentation changes
- `refactor`: Code refactoring
- `test`: Test additions/modifications

Example: `feat(Business): add Excel export service`
