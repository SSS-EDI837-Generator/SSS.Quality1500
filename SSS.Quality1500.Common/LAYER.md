# Common Layer (Cross-Cutting Concerns)

## Proposito
**Utilidades transversales** compartidas por todas las capas. No contiene logica de negocio.

## Dependencias
- **NO** depende de ninguna otra capa del proyecto
- Solo depende de paquetes NuGet externos (Serilog, etc.)

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Services/` | Servicios transversales | `LazyService.cs` |
| `Interfaces/` | Contratos transversales | `IEnvironmentProvider.cs` |
| `Extensions/` | Metodos de extension genericos | `StringExtensions.cs`, `ServiceCollectionExtensions.cs` |

## Archivos Clave
- `EnvironmentProvider.cs` - Detecta ambiente (Development/Production)
- `Version.cs` - Informacion de version del ensamblado
- `LazyService.cs` - Wrapper para inyeccion lazy de servicios

**Nota:** `Result<T,E>` ahora está en **Domain/Models/** (es un concepto de dominio, no una utilidad técnica).
**Nota:** `LoggerInitializer` ahora está en **Business/Services/** (inicialización de infraestructura de aplicación).

## Reglas
1. **NO** incluir logica de negocio
2. **NO** incluir constantes de negocio (esas van en Domain)
3. Solo codigo reutilizable y generico
4. Mantener dependencias externas al minimo

## Ejemplo de Utilidad Transversal
```csharp
// EnvironmentProvider - Detecta el ambiente de ejecución
public class EnvironmentProvider : IEnvironmentProvider
{
    public string GetEnvironment()
    {
#if DEBUG
        return "Development";
#else
        return "Production";
#endif
    }
}

// Uso
var envProvider = new EnvironmentProvider();
string env = envProvider.GetEnvironment(); // "Development" o "Production"
```
