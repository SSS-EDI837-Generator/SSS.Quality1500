# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview
**SSS.Quality1500** is a WPF desktop application built with .NET 10 using Clean Architecture and strict MVVM. It is designed for healthcare claims quality review (CMS-1500 forms).

## Build and Run Commands

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
```

## Architecture: Onion Architecture with 5 Layers

The codebase follows **Onion Architecture** (also known as Clean Architecture / Hexagonal Architecture). Dependencies always point **inward** toward the core (Domain).

```
        ┌─────────────────────────────────────┐
        │          PRESENTATION               │  ← Outer layer (UI)
        │  ┌───────────────────────────────┐  │
        │  │          BUSINESS             │  │  ← Application/Use Cases
        │  │  ┌─────────────────────────┐  │  │
        │  │  │          DATA           │  │  │  ← Infrastructure
        │  │  │  ┌───────────────────┐  │  │  │
        │  │  │  │      DOMAIN       │  │  │  │  ← Core (no dependencies)
        │  │  │  └───────────────────┘  │  │  │
        │  │  └─────────────────────────┘  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘

        Dependencies → always point inward
```

**Dependency Chain:**
```
Presentation → Business → Data → Domain ← Common
```

### Layer Responsibilities

| Layer | Purpose | Key Rules |
|-------|---------|-----------|
| **Common** | Cross-cutting utilities (EnvironmentProvider, Version, LazyService) | NO business logic, NO dependency on other project layers |
| **Domain** | Entities, value objects, `Result<T,E>`, interfaces (contracts) | NO framework dependencies, NO async in entities, defines WHAT not HOW |
| **Data** | Implements Domain contracts (DBF readers via NDbfReader, SQLite via EF Core) | Implements interfaces from Domain, handles infrastructure exceptions |
| **Business** | Orchestrates use cases, contains DTOs, mappers, and LoggerInitializer | Orchestrates (not implements) data access, NO UI dependencies |
| **Presentation** | WPF UI with MVVM pattern | Strict MVVM with NO code-behind (except `InitializeComponent`) |

### Critical Architectural Rules

1. **Domain defines contracts, Data implements them**
   - Interfaces go in `Domain/Interfaces/`
   - Implementations go in `Data/Services/` or `Data/Repositories/`
   - Use `using Domain.Interfaces`, NOT `using Data.Interfaces`

2. **DTOs vs ViewModels**
   - DTOs (Business/Models): Use `List<T>`, no `ObservableCollection`, no UI types
   - ViewModels (Presentation/Models): Can use `ObservableCollection<T>`, `INotifyPropertyChanged`
   - Domain entities: Pure objects with business rules, no UI bindings

3. **Result Pattern** (from Domain layer)
   - Use `Result<TSuccess, TFailure>` instead of exceptions for expected failures
   - Result is a domain concept, not a utility - models success/failure in business operations
   - Convert infrastructure exceptions to Result in Data layer

4. **Dependency Injection**
   - Each layer has `Extensions/ServiceCollectionExtensions.cs`
   - Chain: `AddCommonServices()` → `AddDataServices(config)` → `AddBusinessServices(config)` → `AddPresentationServices(config)`

5. **No Redundant Transitive References**
   - Each project should only reference its **immediate dependency**, not transitive ones
   - .NET propagates dependencies automatically through the chain
   - This keeps `.csproj` files clean and the dependency graph explicit

## MVVM Implementation

### Required Patterns
- Use `[ObservableProperty]` and `[RelayCommand]` from CommunityToolkit.Mvvm
- Views must only contain `InitializeComponent()` in code-behind
- Navigation and dialogs through services, not direct UI calls
- Binding with `UpdateSourceTrigger=PropertyChanged`

### Example ViewModel Structure
```csharp
public partial class ExampleViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // Business layer returns List<T> (DTO)
        Result<List<MyDto>, string> result = await _service.GetDataAsync();

        // Convert to ObservableCollection for UI binding
        result.OnSuccess(dtos =>
            MyCollection = new ObservableCollection<MyDto>(dtos)
        );
    }
}
```

## Key Patterns

### Aggregates (Domain-Driven Design)
- **Domain Aggregates** (`Domain/Aggregates/`): Group related entities with consistency rules, single root entity
- **Aggregate Services** (`Business/Aggregates/`): Compose multiple services to reduce constructor dependencies
- Contracts in `Aggregates/Abstractions/` folders

### Configuration
- Configuration files: `appsettings.json` and `appsettings.{Environment}.json`
- Settings loaded in `ServiceConfigurator.ConfigureServices()`
- Environment detection via `EnvironmentProvider.GetEnvironment()`

### Logging
- Serilog initialized in `Business/Services/LoggerInitializer.cs`
- Logger configuration in App.xaml.cs constructor (bootstrap)
- LoggerInitializer registered in DI for reuse across the template

## Tech Stack
- **.NET 10** (net10.0-windows for Presentation layer)
- **WPF** with MaterialDesignThemes 5.x
- **CommunityToolkit.Mvvm** for MVVM infrastructure
- **NDbfReader** for DBF file access
- **EF Core + SQLite** for persistence
- **Serilog** for logging

## Code Conventions
- File-scoped namespaces
- Primary constructors when applicable
- Explicit types for simple types (avoid `var`)
- Maximum 500 lines per .cs file (excluding auto-generated)
- Maximum 3 parameters in constructors/methods

## Architectural Compliance Rules (STRICT)

### Dependency Matrix - What Each Layer Can Use

| Layer | Can Use | CANNOT Use | Namespace Pattern |
|-------|---------|------------|-------------------|
| **Common** | NuGet packages only | Domain, Data, Business, Presentation | `SSS.Quality1500.Common.*` |
| **Domain** | Nothing (pure core) | Common, Data, Business, Presentation | `SSS.Quality1500.Domain.*` |
| **Data** | Domain, Common | Business, Presentation | `SSS.Quality1500.Data.*` |
| **Business** | Domain, Data, Common | Presentation | `SSS.Quality1500.Business.*` |
| **Presentation** | Domain, Business, Common | Data (use Business instead) | `SSS.Quality1500.Presentation.*` |

### Dependency Flow Diagram (Onion Architecture)

```
┌─────────────┐         ┌─────────────┐
│   Common    │         │   Domain    │
│  (Utilities)│         │   (Core)    │
│ - Version   │         │ - Result<T> │
│ - EnvProvider│        │ - Entities  │
│ - LazyService│        │ - Contracts │
│ (no deps)   │         │ (no deps)   │
└─────────────┘         └─────────────┘
       ↑                       ↑
       │                       │
       └──────────┬────────────┘
                  │
          ┌───────────────┐
          │     Data      │
          │(Infrastructure)│
          │ - DbfReader   │
          │ - Repositories│
          └───────────────┘
                  ↑
                  │
          ┌───────────────┐
          │   Business    │
          │ (Application) │
          │ - Services    │
          │ - DTOs        │
          │ - Logger      │
          └───────────────┘
                  ↑
                  │
          ┌───────────────┐
          │ Presentation  │
          │     (UI)      │
          │ - ViewModels  │
          │ - Views       │
          └───────────────┘

    ↑ = dependency direction (always toward center)
```

### Onion Architecture Principles

| Principle | Implementation |
|-----------|----------------|
| **Domain at center** | Domain has 0 ProjectReferences |
| **Dependencies inward** | Outer layers depend on inner layers only |
| **Dependency inversion** | Domain defines interfaces, Data implements |
| **Transitive flow** | Each layer only references its immediate inner layer |

### Critical Violations to Avoid

**❌ NEVER do this:**
```csharp
// ❌ Domain depending on Common
namespace SSS.Quality1500.Domain.Constants;
using SSS.Quality1500.Common.Something; // VIOLATION

// ❌ Data depending on Business
namespace SSS.Quality1500.Data.Services;
using SSS.Quality1500.Business.Models; // VIOLATION

// ❌ Business depending on Presentation
namespace SSS.Quality1500.Business.Services;
using SSS.Quality1500.Presentation.ViewModels; // VIOLATION

// ❌ File in Domain/ using Common namespace
// File location: Domain/Constants/MyConstants.cs
namespace SSS.Quality1500.Common.Constants; // VIOLATION - namespace must match folder
```

**✅ CORRECT patterns:**
```csharp
// ✅ Domain defines contract
namespace SSS.Quality1500.Domain.Interfaces;
public interface IDataService { }

// ✅ Data implements Domain contract
namespace SSS.Quality1500.Data.Services;
using SSS.Quality1500.Domain.Interfaces;
public class DataService : IDataService { }

// ✅ Business uses Domain and Data
namespace SSS.Quality1500.Business.Services;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Data.Services;

// ✅ Namespace matches physical location
// File: Domain/Constants/MyConstants.cs
namespace SSS.Quality1500.Domain.Constants; // ✅ CORRECT
```

### No Redundant Transitive References (ProjectReference)

Each `.csproj` must only reference its **immediate dependency**. Types from deeper layers are available transitively.

**Dependency Chain:**
```
Presentation → Business → Data → Domain
                                    ↑
                    available transitively to all
```

**❌ WRONG - Redundant references in .csproj:**
```xml
<!-- Presentation.csproj - WRONG -->
<ItemGroup>
  <ProjectReference Include="..\Business\Business.csproj" />
  <ProjectReference Include="..\Domain\Domain.csproj" />  <!-- ❌ REDUNDANT -->
</ItemGroup>

<!-- Business.csproj - WRONG -->
<ItemGroup>
  <ProjectReference Include="..\Data\Data.csproj" />
  <ProjectReference Include="..\Domain\Domain.csproj" />  <!-- ❌ REDUNDANT -->
</ItemGroup>
```

**✅ CORRECT - Only immediate dependencies:**
```xml
<!-- Presentation.csproj - CORRECT -->
<ItemGroup>
  <ProjectReference Include="..\Business\Business.csproj" />
  <!-- Domain types available via Business → Data → Domain -->
</ItemGroup>

<!-- Business.csproj - CORRECT -->
<ItemGroup>
  <ProjectReference Include="..\Data\Data.csproj" />
  <!-- Domain types available via Data → Domain -->
</ItemGroup>

<!-- Data.csproj - CORRECT (references Domain directly as immediate dependency) -->
<ItemGroup>
  <ProjectReference Include="..\Domain\Domain.csproj" />
  <ProjectReference Include="..\Common\Common.csproj" />
</ItemGroup>
```

**Current valid ProjectReferences:**

| Project | Direct References | Gets Transitively |
|---------|-------------------|-------------------|
| Domain | (none) | - |
| Common | (none) | - |
| Data | Domain, Common | - |
| Business | Data | Domain, Common |
| Presentation | Business | Data, Domain, Common |

### Verification Checklist

Before committing code, verify:
- [ ] `.csproj` files only reference **immediate** dependencies (no transitive redundancy)
- [ ] `using` statements follow dependency rules
- [ ] Namespaces match physical folder structure
- [ ] Domain has NO dependencies on other project layers
- [ ] Common has NO dependencies on other project layers
- [ ] `dotnet build` succeeds with 0 warnings

**Quick .csproj reference check:**
- `Presentation.csproj` → only `Business`
- `Business.csproj` → only `Data`
- `Data.csproj` → `Domain` + `Common`
- `Domain.csproj` → (none)
- `Common.csproj` → (none)

### Template Design Note

This is a **reusable template** for creating new WPF applications. Therefore:
- Services may exist in layers even if not currently used in this template instance
- Interfaces like `ILoggerInitializer` enable DI flexibility across different template usages
- LoggerInitializer is in Business (not Common) to allow using `Result<T>` from Domain

## Documentation
Each layer has a detailed `LAYER.md` file with specific guidance, examples, and rules. Always consult the relevant LAYER.md when working in a specific layer.

## Documentación Complementaria

Para contexto específico de healthcare y validaciones, consulta los documentos en `docs/`:

- `docs/HEALTHCARE_CONTEXT.md` - Contexto de CMS-1500 y healthcare
- `docs/VALIDATION_RULES.md` - Reglas de validación de negocio
- `docs/API_INTEGRATION.md` - Integración con API de NPI
- `docs/UI_PATTERNS.md` - Patrones de UI para revisión visual