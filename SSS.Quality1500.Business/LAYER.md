# Business Layer (Application Services)

## Proposito
**Orquestacion y casos de uso**. Coordina las entidades del Domain con las implementaciones de Data para ejecutar flujos de negocio completos.

## Dependencias
- Depende de **Domain** (entidades, contratos)
- Depende de **Data** (implementaciones de repositorios)
- Depende de **Common** (utilidades transversales)
- **NO** depende de Presentation

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Services/` | Casos de uso / Application Services | `VdeRecordService.cs` |
| `Services/Interfaces/` | Contratos de servicios de negocio | `IVdeRecordService.cs` |
| `Models/` | DTOs de entrada/salida (NO ViewModels) | `VdeRecord.cs`, `StartProcessRequest.cs` |
| `Mappers/` | Transformadores DataTable → DTOs | `VdeRecordMapper.cs` |
| `Events/` | Eventos de aplicacion | `ProgressEvent.cs` |
| `Aggregates/` | ⚠️ Servicios agregados (actualmente vacio) | Ver seccion "Aggregate Services" |
| `Aggregates/Abstractions/` | Interfaces de servicios agregados | `IClaimProcessingAggregate.cs` |
| `Extensions/` | Extensiones y registro DI | `ServiceCollectionExtensions.cs` |

## Reglas
1. Los servicios de Business **orquestan**, no implementan acceso a datos
2. Usar inyeccion de dependencias para obtener repositorios
3. Validaciones de contexto (que dependen de configuracion/usuario) van aqui
4. Registrar servicios en `ServiceCollectionExtensions.cs`

## Ejemplo de Servicio
```csharp
// Business/Services/VdeRecordService.cs
public class VdeRecordService(IDbfReader dbfReader) : IVdeRecordService
{
    public async Task<Result<List<VdeRecord>, string>> GetAllAsVdeRecordsAsync(string filePath)
    {
        Result<DataTable, string> dataTableResult = await dbfReader.GetAllAsDataTableAsync(filePath);
        
        return dataTableResult.Map(dataTable =>
            VdeRecordMapper.MapDataTableToVdeRecords(dataTable)
        );
    }
}
```

## DTOs vs ViewModels

### ¿Donde van los modelos?

| Tipo | Ubicacion | Proposito | Ejemplo |
|------|-----------|-----------|--------|
| **DTO** | `Business/Models/` | Transferir datos entre capas, sin logica de UI | `VdeRecord.cs`, `ProcessingResult.cs` |
| **ViewModel** | `Presentation/Models/` | Binding de WPF, puede usar `ObservableCollection` | `VkFileRecord.cs` |
| **Entity** | `Domain/Models/` | Reglas de negocio intrinsecas | `BatchRecord.cs` |

### Reglas
❌ **Business/Models NO debe contener:**
- `ObservableCollection<T>` (es de WPF)
- `INotifyPropertyChanged` (es de UI)
- `DataAnnotations` para validacion de UI

✅ **Business/Models SI debe usar:**
- `List<T>` o `IEnumerable<T>`
- Propiedades simples (get/set o init)
- Metodos de validacion de negocio si necesario

## Aggregate Services (Patron de Composicion)

### ¿Que son?
Un **Aggregate Service** combina multiples servicios en uno solo para reducir dependencias del constructor. NO es lo mismo que un "Domain Aggregate" (DDD).

### ¿Cuando usar Aggregate Services?
✅ **USA Aggregate Services cuando:**
- Un ViewModel necesita inyectar 5+ servicios (viola Single Responsibility)
- Multiples ViewModels usan el mismo grupo de servicios juntos
- Quieres simplificar el constructor de consumidores

❌ **NO uses Aggregate Services cuando:**
- Solo necesitas 1-3 servicios (inyectalos directamente)
- Los servicios no se usan juntos frecuentemente

### Ejemplo de Aggregate Service
```csharp
// Business/Aggregates/Abstractions/IClaimProcessingAggregate.cs
public interface IClaimProcessingAggregate
{
    IDbfReader DbfReader { get; }
    IVdeRecordService VdeService { get; }
    IReportGenerator ReportGenerator { get; }
    IValidator Validator { get; }
}

// Business/Aggregates/ClaimProcessingAggregate.cs
public class ClaimProcessingAggregate(
    IDbfReader dbfReader,
    IVdeRecordService vdeService,
    IReportGenerator reportGenerator,
    IValidator validator) : IClaimProcessingAggregate
{
    public IDbfReader DbfReader { get; } = dbfReader;
    public IVdeRecordService VdeService { get; } = vdeService;
    public IReportGenerator ReportGenerator { get; } = reportGenerator;
    public IValidator Validator { get; } = validator;
}

// En ViewModel (antes: 4 parametros, despues: 1)
public class ClaimViewModel(IClaimProcessingAggregate aggregate)
{
    public async Task LoadAsync()
    {
        var data = await aggregate.DbfReader.GetAllAsDataTableAsync("file.dbf");
        var records = await aggregate.VdeService.GetAllAsVdeRecordsAsync("file.dbf");
        var validation = aggregate.Validator.Validate(records);
        await aggregate.ReportGenerator.GenerateAsync(validation);
    }
}
```

### Diferencia: Domain Aggregate vs Aggregate Service

| Aspecto | Domain Aggregate (DDD) | Aggregate Service (Composicion) |
|---------|------------------------|----------------------------------|
| Ubicacion | `Domain/Aggregates/` | `Business/Aggregates/` |
| Proposito | Encapsular entidades relacionadas | Agrupar servicios relacionados |
| Ejemplo | Order + OrderLines | DbfReader + Validator + Reporter |
| Logica | Reglas de consistencia entre entidades | Solo composicion, sin logica |

## Registro DI
```csharp
public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration config)
{
    services.AddDataServices(config); // Primero Data
    
    // Servicios de Business
    services.AddTransient<IVdeRecordService, VdeRecordService>();
    services.AddSingleton<IEventAggregator, EventAggregator>();
    
    // Aggregate Services (si los usas)
    services.AddTransient<IClaimProcessingAggregate, ClaimProcessingAggregate>();
    
    return services;
}
```
