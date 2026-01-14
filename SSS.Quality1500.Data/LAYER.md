# Data Layer (Infrastructure)

## Proposito
**Implementacion de acceso a datos**. Contiene las implementaciones concretas de los contratos definidos en Domain (repositorios, lectores de archivos, etc.).

## Dependencias
- Depende de **Domain** (implementa sus contratos/interfaces)
- Depende de **Common** (utilidades transversales)
- **NO** depende de Business ni Presentation
- **SI** puede usar frameworks externos (EF Core, NDbfReader, etc.)

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Services/` | Implementaciones de contratos de Domain | `DbfReader.cs` |
| `Repositories/` | Implementaciones de repositorios | `SqlClaimRepository.cs` |
| `Context/` | DbContext de Entity Framework | `AppDbContext.cs` |
| `Configuration/` | Configuracion de EF (Fluent API) | `ClaimConfiguration.cs` |
| `Models/` | Modelos de persistencia (si difieren de Domain) | `ClaimEntity.cs` |
| `Extensions/` | Registro DI | `ServiceCollectionExtensions.cs` |

## Reglas
1. Implementar interfaces definidas en **Domain**, no crear nuevas
2. Los using deben ser `Domain.Interfaces`, no `Data.Interfaces`
3. Manejar excepciones de infraestructura y convertirlas a `Result<T,E>`
4. Registrar implementaciones en `ServiceCollectionExtensions.cs`

## Ejemplo de Implementacion
```csharp
// Data/Services/DbfReader.cs
using SSS.Project.Domain.Interfaces; // Contrato de Domain

public class DbfReader : IDbfReader // Implementa contrato de Domain
{
    public async Task<Result<DataTable, string>> GetAllAsDataTableAsync(string path)
    {
        try { ... }
        catch (Exception ex) { return Result<DataTable, string>.Fail(ex.Message); }
    }
}
```

## Registro DI
```csharp
public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration config)
{
    // IDbfReader es de Domain, DbfReader es de Data
    services.AddTransient<IDbfReader, DbfReader>();
    return services;
}
```
