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

**Dependency Chain (Onion Architecture):**
```
         ┌──────────┐
         │  Domain  │  ← Core (no dependencies)
         └──────────┘
              ↑
   ┌──────────┴──────────┐
   │                     │
┌────────┐          ┌─────────┐
│Business│          │  Data   │
│        │          │         │
└────────┘          └─────────┘
   ↑                     ↑
   └──────────┬──────────┘
              │
       ┌─────────────┐
       │ Presentation│  ← Composition Root
       └─────────────┘
```

### Layer Responsibilities

| Layer | Purpose | Key Rules |
|-------|---------|-----------|
| **Common** | Cross-cutting utilities (EnvironmentProvider, Version, LazyService) | NO business logic, NO dependency on other project layers |
| **Domain** | Entities, value objects, `Result<T,E>`, interfaces (contracts) | NO framework dependencies, NO async in entities, defines WHAT not HOW |
| **Data** | Implements Domain contracts (DBF readers via NDbfReader, SQLite via EF Core) | Implements interfaces from Domain, handles infrastructure exceptions |
| **Business** | Orchestrates use cases, contains DTOs, mappers, and LoggerInitializer | **Only depends on Domain**, NO Data/Common/Presentation dependencies |
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
   - **Presentation is the Composition Root** - it registers all services
   - In `AddPresentationServices`: `AddDataServices(config)` → `AddBusinessServices(config)` → `AddCommonServices()`
   - Business does NOT call AddDataServices (would violate Onion Architecture)

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
| **Business** | **Domain only** | Common, Data, Presentation | `SSS.Quality1500.Business.*` |
| **Presentation** | Business, Data (Composition Root) | - | `SSS.Quality1500.Presentation.*` |

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
       │               ┌───────┴───────┐
       │               │               │
       │       ┌───────────────┐  ┌───────────────┐
       └───────│     Data      │  │   Business    │
               │(Infrastructure)│  │ (Application) │
               │ - DbfReader   │  │ - Handlers    │
               │ - Repositories│  │ - DTOs        │
               └───────────────┘  └───────────────┘
                       ↑                 ↑
                       │                 │
                       └────────┬────────┘
                                │
                       ┌───────────────┐
                       │ Presentation  │
                       │(Composition   │
                       │    Root)      │
                       │ - ViewModels  │
                       │ - DI Wiring   │
                       └───────────────┘

    ↑ = dependency direction
    Business → Domain only (uses interfaces)
    Data → Domain + Common (implements interfaces)
    Presentation → Business + Data (wires everything)
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

// ✅ Business uses ONLY Domain interfaces (not Data implementations)
namespace SSS.Quality1500.Business.Handlers;
using SSS.Quality1500.Domain.Interfaces;
public class MyHandler(IDataService dataService) { } // Injected via DI

// ✅ Presentation wires Data implementations to Domain interfaces
namespace SSS.Quality1500.Presentation.Extensions;
services.AddTransient<IDataService, DataService>(); // Data → Domain
services.AddTransient<MyHandler>();                  // Business uses Domain

// ✅ Namespace matches physical location
// File: Domain/Constants/MyConstants.cs
namespace SSS.Quality1500.Domain.Constants; // ✅ CORRECT
```

### ProjectReference Rules (Onion Architecture)

In Onion Architecture, **Business and Data are sibling layers** - both depend on Domain, but NOT on each other.
**Presentation is the Composition Root** and needs to reference both Business and Data to wire dependencies.

**❌ WRONG - Business depending on Data:**
```xml
<!-- Business.csproj - WRONG (violates Onion Architecture) -->
<ItemGroup>
  <ProjectReference Include="..\Data\Data.csproj" />  <!-- ❌ VIOLATION -->
</ItemGroup>
```

**✅ CORRECT - Onion Architecture:**
```xml
<!-- Domain.csproj - Core, no dependencies -->
<ItemGroup>
  <!-- (none) -->
</ItemGroup>

<!-- Common.csproj - Utilities, no dependencies -->
<ItemGroup>
  <!-- (none) -->
</ItemGroup>

<!-- Data.csproj - Implements Domain interfaces -->
<ItemGroup>
  <ProjectReference Include="..\Domain\Domain.csproj" />
  <ProjectReference Include="..\Common\Common.csproj" />
</ItemGroup>

<!-- Business.csproj - Uses Domain interfaces ONLY -->
<ItemGroup>
  <ProjectReference Include="..\Domain\Domain.csproj" />
</ItemGroup>

<!-- Presentation.csproj - Composition Root, wires everything -->
<ItemGroup>
  <ProjectReference Include="..\Business\Business.csproj" />
  <ProjectReference Include="..\Data\Data.csproj" />
</ItemGroup>
```

**Current valid ProjectReferences:**

| Project | Direct References | Gets Transitively |
|---------|-------------------|-------------------|
| Domain | (none) | - |
| Common | (none) | - |
| Data | Domain, Common | - |
| Business | **Domain only** | - |
| Presentation | Business, Data | Domain, Common |

### Verification Checklist

Before committing code, verify:
- [ ] `.csproj` files only reference **immediate** dependencies (no transitive redundancy)
- [ ] `using` statements follow dependency rules
- [ ] Namespaces match physical folder structure
- [ ] Domain has NO dependencies on other project layers
- [ ] Common has NO dependencies on other project layers
- [ ] `dotnet build` succeeds with 0 warnings

**Quick .csproj reference check:**
- `Presentation.csproj` → `Business` + `Data` (Composition Root)
- `Business.csproj` → `Domain` only (NO Data reference!)
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