# Domain Layer

## Proposito
El **corazon del negocio**. Contiene las entidades, reglas de negocio intrinsecas y contratos (interfaces) que definen QUE hace el sistema, sin saber COMO se implementa.

## Dependencias
- Solo depende de **Common** (para tipos transversales como `Result<T,E>`)
- **NO** depende de Data, Business ni Presentation
- **NO** debe tener referencias a frameworks externos (EF, HTTP, UI)

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Constants/` | Constantes de negocio (campos DBF, codigos, etc.) | `ProjectConstants.cs` |
| `Entities/` | Entidades con identidad unica | `ClaimRecord.cs`, `Patient.cs` |
| `ValueObjects/` | Objetos inmutables sin identidad | `Money.cs`, `NPI.cs`, `Address.cs` |
| `Interfaces/` | Contratos/abstracciones (repositorios, servicios) | `IDbfReader.cs`, `IClaimRepository.cs` |
| `Extensions/` | Metodos de extension para entidades/VOs | `ClaimExtensions.cs` |
| `Helpers/` | Utilidades especificas del dominio | `ValidationHelper.cs` |

## Reglas
1. Las entidades deben validar sus propias reglas intrinsecas
2. Los contratos (interfaces) definen QUE se necesita, no COMO se obtiene
3. No usar `async/await` en entidades (son objetos de datos puros)
4. Value Objects deben ser inmutables (usar `init` o constructor)

## Ejemplo de Entidad
```csharp
public sealed class ClaimRecord
{
    public string Document { get; init; } = string.Empty;
    public int Sequence { get; init; }
    
    // Regla de negocio intrinseca
    public bool IsValid() => !string.IsNullOrWhiteSpace(Document) && Sequence > 0;
}
```

## Ejemplo de Contrato
```csharp
public interface IClaimRepository
{
    Task<Result<ClaimRecord, string>> GetByIdAsync(string id);
    Task<IReadOnlyList<ClaimRecord>> GetAllAsync();
}
```
