# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project overview
SSS.Quality1500 is a WPF desktop app for claims quality review (CMS-1500). It follows a Clean Architecture-style layering model and uses strict MVVM in the WPF Presentation layer.

## Commands
### Restore / build
```bash
# Restore all projects (solution is in .slnx format)
dotnet restore SSS.Quality1500.slnx

# Build everything (includes the WPF Presentation project)
dotnet build SSS.Quality1500.slnx

# Build Release
dotnet build SSS.Quality1500.slnx -c Release

# Build only the non-UI layers (useful in CI or when you don't need WPF)
dotnet build SSS.Quality1500.Domain/SSS.Quality1500.Domain.csproj
dotnet build SSS.Quality1500.Data/SSS.Quality1500.Data.csproj
dotnet build SSS.Quality1500.Business/SSS.Quality1500.Business.csproj
```

### Run (WPF)
```bash
dotnet run --project SSS.Quality1500.Presentation/SSS.Quality1500.Presentation.csproj
```
Note: `SSS.Quality1500.Presentation` targets `net10.0-windows` (`<UseWPF>true</UseWPF>`). This is a Windows (WPF) application.
- Running the UI requires Windows.
- If you are on a non-Windows machine, focus on building the non-UI projects (Domain/Data/Business).

### Tests
There are currently no `*.Tests.csproj` projects in this repo.

If/when tests are added:
```bash
# Run all tests in the repo
dotnet test

# Run tests in a specific test project
dotnet test path/to/SomeProject.Tests/SomeProject.Tests.csproj

# Run a single test (MSTest/xUnit/NUnit) by name pattern
dotnet test path/to/SomeProject.Tests/SomeProject.Tests.csproj --filter FullyQualifiedName~Namespace.TypeName.TestName
```

### “Lint” / static checks
This repo does not currently define a separate linting toolchain (no `.editorconfig` / tool manifest). The main quality gate is compilation.

```bash
# Treat warnings as errors (useful as a stricter CI check)
dotnet build SSS.Quality1500.slnx -p:TreatWarningsAsErrors=true
```

### Clean
```bash
dotnet clean SSS.Quality1500.slnx
```

## Architecture
### Dependency flow (innermost to outermost)
```
Common <- Domain <- Data <- Business <- Presentation
```

### Solution entry points (where to start reading)
- Solution file: `SSS.Quality1500.slnx`
- Composition root / startup: `SSS.Quality1500.Presentation/App.xaml.cs`
  - Bootstraps logging (`SSS.Quality1500.Common/LoggerInitializer.cs`)
  - Picks the configuration environment via `SSS.Quality1500.Common/EnvironmentProvider.cs` (`Development` in DEBUG, otherwise `Production`)
  - Builds the DI container via `SSS.Quality1500.Presentation/Configuration/ServiceConfigurator.cs`
- DI registration “hub”: `SSS.Quality1500.Presentation/Extensions/ServiceCollectionExtensions.cs`
  - `AddPresentationServices(configuration)` composes Presentation + Business + Data + Common registrations.

### Configuration model
- Runtime config is loaded from the application base directory (output folder), not the repo root:
  - `SSS.Quality1500.Presentation/appsettings.json`
  - `SSS.Quality1500.Presentation/appsettings.Development.json`
- The `.csproj` marks them as `<Content>` and copies them to the output directory (see `SSS.Quality1500.Presentation/SSS.Quality1500.Presentation.csproj`).
- Relevant sections:
  - `ApplicationSettings` (bound to `SSS.Quality1500.Presentation/Models/ApplicationSettings.cs` via `IOptions<T>`)
  - `ApiEndpoints` uses placeholder tokens (e.g. `#{...}#`) and is also overridable via environment variables.

### Data flow: DBF -> DataTable -> DTO -> UI
- Domain contract: `SSS.Quality1500.Domain/Interfaces/IDbfReader.cs`
- Data implementation: `SSS.Quality1500.Data/Services/DbfReader.cs` (NDbfReader)
  - Populates `TotalImages`, `TotalClaims`, and `Bht` as side data while reading.
- Business adapter: `SSS.Quality1500.Business/Services/VdeRecordService.cs`
  - Wraps `IDbfReader` and maps `DataTable -> List<VdeRecord>` via `SSS.Quality1500.Business/Mappers/VdeRecordMapper.cs`.
- DBF field constants are centralized in:
  - `SSS.Quality1500.Domain/Constants/ProjectConstants.cs`
  - `SSS.Quality1500.Domain/Constants/VdeConstantsUpdated.cs` (note: its namespace is `SSS.Quality1500.Common.Constants` even though the file lives under `Domain/Constants`).

### Cross-layer messaging (progress/events)
- Domain defines the eventing contracts: `SSS.Quality1500.Domain/Interfaces/IEventAggregator.cs`
- Business provides the concrete implementation: `SSS.Quality1500.Business/Services/EventAggregator.cs`
- UI subscribes and marshals updates onto the UI thread:
  - `SSS.Quality1500.Presentation/Services/ProgressEventHandlerService.cs`
  - Handlers live in `SSS.Quality1500.Presentation/EventHandlers/` and use `Dispatcher`.

### MVVM in Presentation
- Views are in `SSS.Quality1500.Presentation/Views/` and do *only* `InitializeComponent()` + `DataContext = viewModel` in code-behind.
- ViewModels are in `SSS.Quality1500.Presentation/ViewModels/`.
- Prefer DI for creating Views/ViewModels (see registrations in `Presentation/Extensions/ServiceCollectionExtensions.cs`).

### Aggregate services to keep constructors small
Some Presentation services intentionally group related dependencies to keep consumer constructors under the “3 parameters” convention:
- `SSS.Quality1500.Presentation/Interfaces/IConfigurationServices.cs`
- `SSS.Quality1500.Presentation/Services/ConfigurationServices.cs`

`ConfigurationServices` also uses `LazyService<T>` (from `SSS.Quality1500.Common/Services/LazyService.cs`) for expensive/optional dependencies.

## Code conventions used across the repo
- File-scoped namespaces
- Primary constructors where they improve clarity
- Explicit types for simple types where the type is not self-evident
- Keep constructors/methods at <= 3 parameters (use aggregate services when necessary)
- Keep each `.cs` file <= 500 lines (excluding autogenerated files)
