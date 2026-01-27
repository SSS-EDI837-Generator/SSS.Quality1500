# Arquitectura: Onion + CQRS

Este documento describe la arquitectura del proyecto para que desarrolladores y herramientas de IA (Claude Code, Copilot, etc.) puedan entender y aplicar los patrones correctamente.

---

## Onion Architecture

### Principio fundamental

> **Las dependencias apuntan hacia el centro. Las capas internas NO conocen a las externas.**

```
            ┌─────────────────────────────────────────┐
            │   Presentation + Data (EXTERIOR)        │
            │  ┌───────────────────────────────────┐  │
            │  │           Business                │  │
            │  │  ┌─────────────────────────────┐  │  │
            │  │  │     Domain (CENTRO)         │  │  │
            │  │  └─────────────────────────────┘  │  │
            │  └───────────────────────────────────┘  │
            └─────────────────────────────────────────┘
```

### Capas y dependencias

| Capa | Depende de | NO puede depender de | Responsabilidad |
|------|-----------|---------------------|-----------------|
| **Domain** | Nada | Business, Data, Presentation | Entidades, interfaces, Result<T,E> |
| **Business** | Domain | Data, Presentation | Casos de uso, DTOs, Queries, Commands |
| **Data** | Domain | Business, Presentation | Implementa interfaces de Domain |
| **Presentation** | Business, Data | - | UI + Composition Root (DI) |

### Regla de oro

```
Domain:       NO conoce a nadie (es el centro puro)
Business:     Solo conoce Domain (usa interfaces, NO implementaciones)
Data:         Conoce Domain (implementa sus interfaces)
Presentation: Conoce Business y Data (para DI registration)
```

### Ejemplo de dependencias correctas

```csharp
// ✅ CORRECTO: Domain define interfaz
namespace Project.Domain.Interfaces;
public interface IDbfReader
{
    Task<Result<DataTable, string>> GetAllAsync(string path);
}

// ✅ CORRECTO: Data implementa interfaz de Domain
namespace Project.Data.Services;
using Project.Domain.Interfaces;
public class DbfReader : IDbfReader { ... }

// ✅ CORRECTO: Business usa interfaz de Domain (NO conoce Data)
namespace Project.Business.Services;
using Project.Domain.Interfaces;
public class MyService(IDbfReader reader) { ... }

// ✅ CORRECTO: Presentation registra implementación en DI
namespace Project.Presentation.Extensions;
using Project.Domain.Interfaces;
using Project.Data.Services;
services.AddTransient<IDbfReader, DbfReader>();
```

### Ejemplo de violaciones (EVITAR)

```csharp
// ❌ INCORRECTO: Business conoce Data directamente
namespace Project.Business.Services;
using Project.Data.Services;  // VIOLACIÓN
public class MyService(DbfReader reader) { }  // Usa implementación, no interfaz

// ❌ INCORRECTO: Domain depende de otra capa
namespace Project.Domain.Models;
using Project.Business.Models;  // VIOLACIÓN
```

---

## CQRS (Command Query Responsibility Segregation)

### Principio fundamental

> **Separar operaciones de lectura (Queries) de operaciones de escritura (Commands).**

### Estructura de carpetas

```
Business/
├── Queries/                      ← Operaciones de LECTURA
│   └── GetRecords/
│       ├── GetRecordsQuery.cs        (define qué datos necesitas)
│       └── GetRecordsHandler.cs      (ejecuta la consulta)
│
├── Commands/                     ← Operaciones de ESCRITURA
│   └── ProcessData/
│       ├── ProcessDataCommand.cs     (define la acción)
│       └── ProcessDataHandler.cs     (ejecuta la acción)
│
└── Services/                     ← Servicios tradicionales (opcional)
```

### Interfaces base (en Domain/CQRS/)

```csharp
// Query: operación de lectura (no modifica estado)
public interface IQuery<TResult>;
public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}

// Command: operación de escritura (modifica estado)
public interface ICommand<TResult>;
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
```

### Ejemplo de Query (Lectura)

```csharp
// 1. Definir Query
public record GetRecordsQuery(string FilePath)
    : IQuery<Result<List<RecordDto>, string>>;

// 2. Implementar Handler
public class GetRecordsHandler(IDbfReader dbfReader)
    : IQueryHandler<GetRecordsQuery, Result<List<RecordDto>, string>>
{
    public async Task<Result<List<RecordDto>, string>> HandleAsync(
        GetRecordsQuery query, CancellationToken ct = default)
    {
        var result = await dbfReader.GetAllAsync(query.FilePath);
        return result.Map(data => MapToDto(data));
    }
}

// 3. Registrar en DI (Business/Extensions/ServiceCollectionExtensions.cs)
services.AddTransient<IQueryHandler<GetRecordsQuery, Result<List<RecordDto>, string>>,
    GetRecordsHandler>();
```

### Ejemplo de Command (Escritura)

```csharp
// 1. Definir Command
public record ProcessDataCommand(string FilePath, string OutputPath)
    : ICommand<Result<ProcessingResult, string>>;

// 2. Implementar Handler
public class ProcessDataHandler(IDbfReader dbfReader)
    : ICommandHandler<ProcessDataCommand, Result<ProcessingResult, string>>
{
    public async Task<Result<ProcessingResult, string>> HandleAsync(
        ProcessDataCommand cmd, CancellationToken ct = default)
    {
        // Validar
        if (string.IsNullOrWhiteSpace(cmd.FilePath))
            return Result<ProcessingResult, string>.Fail("FilePath required");

        // Procesar
        var data = await dbfReader.GetAllAsync(cmd.FilePath);
        // ... lógica de negocio ...

        return Result<ProcessingResult, string>.Ok(new ProcessingResult(...));
    }
}

// 3. Registrar en DI
services.AddTransient<ICommandHandler<ProcessDataCommand, Result<ProcessingResult, string>>,
    ProcessDataHandler>();
```

### Uso en ViewModels

```csharp
public class MyViewModel(
    IQueryHandler<GetRecordsQuery, Result<List<RecordDto>, string>> getRecordsHandler,
    ICommandHandler<ProcessDataCommand, Result<ProcessingResult, string>> processHandler)
{
    [RelayCommand]
    private async Task LoadData()
    {
        var query = new GetRecordsQuery(FilePath);
        var result = await getRecordsHandler.HandleAsync(query);
        result.OnSuccess(records => Records = new ObservableCollection<RecordDto>(records));
    }

    [RelayCommand]
    private async Task Process()
    {
        var command = new ProcessDataCommand(FilePath, OutputPath);
        var result = await processHandler.HandleAsync(command);
    }
}
```

---

## Result Pattern

### Uso obligatorio

Todas las operaciones que pueden fallar deben retornar `Result<TSuccess, TFailure>`:

```csharp
// Definición (en Domain/Models/Result.cs)
public abstract class Result<TSuccess, TFailure>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }

    public static Result<TSuccess, TFailure> Ok(TSuccess value);
    public static Result<TSuccess, TFailure> Fail(TFailure error);

    public Result<TNew, TFailure> Map<TNew>(Func<TSuccess, TNew> mapper);
    public TResult Match<TResult>(Func<TSuccess, TResult> onSuccess, Func<TFailure, TResult> onFailure);
    public Result<TSuccess, TFailure> OnSuccess(Action<TSuccess> action);
    public Result<TSuccess, TFailure> OnFailure(Action<TFailure> action);
}
```

### Ejemplo de uso

```csharp
Result<DataTable, string> result = await dbfReader.GetAllAsync(path);

// Opción 1: Pattern matching
result.OnSuccess(data => Process(data))
      .OnFailure(error => Log(error));

// Opción 2: Map para transformar
Result<List<Dto>, string> dtos = result.Map(data => MapToDto(data));

// Opción 3: Match para obtener valor
string message = result.Match(
    onSuccess: data => $"Loaded {data.Rows.Count} rows",
    onFailure: error => $"Error: {error}"
);
```

---

## Instrucciones para Claude Code / IA

Cuando trabajes en este proyecto:

### Al crear nuevas funcionalidades

1. **Queries (lectura)**: Crear en `Business/Queries/NombreOperacion/`
2. **Commands (escritura)**: Crear en `Business/Commands/NombreOperacion/`
3. **Interfaces**: Definir en `Domain/Interfaces/`
4. **Implementaciones**: Crear en `Data/Services/`

### Reglas a seguir

- **NUNCA** hacer que Business dependa de Data directamente
- **SIEMPRE** usar interfaces de Domain en Business
- **SIEMPRE** usar `Result<T,E>` para operaciones que pueden fallar
- **SIEMPRE** registrar handlers en `Business/Extensions/ServiceCollectionExtensions.cs`
- **SIEMPRE** registrar implementaciones de Data en `Presentation/Extensions/ServiceCollectionExtensions.cs`

### Estructura de archivos para nueva funcionalidad

```
# Para una nueva operación "ExportReport":

Domain/Interfaces/
└── IReportExporter.cs              ← Interfaz

Data/Services/
└── ReportExporter.cs               ← Implementación

Business/Commands/ExportReport/
├── ExportReportCommand.cs          ← Definición del comando
└── ExportReportHandler.cs          ← Lógica de negocio

Business/Models/
└── ExportResult.cs                 ← DTO de resultado
```

### Checklist antes de terminar

- [ ] Interfaces en Domain, implementaciones en Data
- [ ] Business solo usa interfaces de Domain
- [ ] Queries/Commands registrados en DI
- [ ] Result<T,E> usado para errores esperados
- [ ] `dotnet build` compila sin errores
