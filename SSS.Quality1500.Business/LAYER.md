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
| `Services/` | Casos de uso / Application Services | `ClaimLoadingService.cs` |
| `Aggregates/` | Servicios agregados (patron Aggregate Services) | `ClaimAggregateService.cs` |
| `Aggregates/Abstractions/` | Interfaces de servicios agregados | `IClaimAggregateService.cs` |
| `Events/` | Eventos de aplicacion | `ClaimLoadedEvent.cs` |
| `Models/` | DTOs de entrada/salida | `ClaimLoadRequest.cs` |
| `Extensions/` | Extensiones y registro DI | `ServiceCollectionExtensions.cs` |
| `Helpers/` | Utilidades de orquestacion | `MappingHelper.cs` |

## Reglas
1. Los servicios de Business **orquestan**, no implementan acceso a datos
2. Usar inyeccion de dependencias para obtener repositorios
3. Validaciones de contexto (que dependen de configuracion/usuario) van aqui
4. Registrar servicios en `ServiceCollectionExtensions.cs`

## Ejemplo de Servicio
```csharp
public class ClaimLoadingService(IDbfReader dbfReader)
{
    public async Task<Result<IReadOnlyList<ClaimRecord>, string>> LoadClaimsAsync(string filePath)
    {
        var result = await dbfReader.GetAllAsDataTableAsync(filePath);
        return result.Map(table => MapToEntities(table));
    }
    
    private IReadOnlyList<ClaimRecord> MapToEntities(DataTable table) { ... }
}
```

## Registro DI
```csharp
public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration config)
{
    services.AddDataServices(config); // Primero Data
    services.AddTransient<ClaimLoadingService>();
    return services;
}
```
