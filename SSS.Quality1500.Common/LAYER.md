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
| `Interfaces/` | Contratos transversales | `ILoggerInitializer.cs` |
| `Extensions/` | Metodos de extension genericos | `StringExtensions.cs` |
| `Models/` | DTOs compartidos | `PaginationResult.cs` |

## Archivos Clave
- `Result.cs` - Patron Result para manejo de errores sin excepciones
- `LoggerInitializer.cs` - Configuracion de Serilog
- `EnvironmentProvider.cs` - Detecta ambiente (Development/Production)
- `Version.cs` - Informacion de version del ensamblado

## Reglas
1. **NO** incluir logica de negocio
2. **NO** incluir constantes de negocio (esas van en Domain)
3. Solo codigo reutilizable y generico
4. Mantener dependencias externas al minimo

## Patron Result
```csharp
// Uso correcto del patron Result
Result<DataTable, string> result = await service.GetDataAsync();

result
    .OnSuccess(data => ProcessData(data))
    .OnFailure(error => LogError(error));

// O con Match
string message = result.Match(
    onSuccess: data => "Datos cargados",
    onFailure: error => error
);
```
