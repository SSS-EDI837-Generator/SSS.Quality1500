# Domain Layer

## Proposito
El **corazon del negocio**. Contiene las entidades, reglas de negocio intrinsecas y contratos (interfaces) que definen QUE hace el sistema, sin saber COMO se implementa.

## Dependencias
- **NO** depende de ninguna otra capa del proyecto (es el núcleo puro)
- **NO** depende de Data, Business, Presentation ni Common
- **NO** debe tener referencias a frameworks externos (EF, HTTP, UI)
- Define `Result<T,E>` como concepto de dominio (no es una utilidad técnica)

## Estructura de Carpetas

| Carpeta | Contenido | Ejemplo |
|---------|-----------|--------|
| `Models/` | Entidades del dominio y conceptos fundamentales | `BatchRecord.cs`, `ConfigurationSystem.cs`, **`Result.cs`** |
| `Enums/` | Enumeraciones del dominio | `ProcessingStatus.cs`, `ClaimType.cs` |
| `Interfaces/` | Contratos/abstracciones (repositorios, servicios) | `IDbfReader.cs`, `IEventAggregator.cs`, `ILoggerInitializer.cs` |
| `Constants/` | Constantes de negocio (campos DBF, codigos) | `VdeConstants.cs` |
| `Aggregates/` | ⚠️ Agregados DDD (actualmente vacio) | Ver seccion "Aggregates" |
| `Aggregates/Abstractions/` | Contratos de agregados | `IClaimAggregate.cs` |

## Archivos Fundamentales

### `Result<TSuccess, TFailure>` (Models/Result.cs)
**Por qué está en Domain (no en Common):**
- `Result` es un **concepto de dominio**, no una utilidad técnica
- Modela el lenguaje ubicuo: "toda operación puede tener éxito o fallar"
- Define el contrato de TODAS las operaciones del sistema
- Domain no depende de nadie (es el núcleo puro)

```csharp
// Uso en contratos de Domain
public interface IDbfReader
{
    Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath);
}

// Uso en implementaciones (Data, Business, Presentation)
Result<DataTable, string> result = await dbfReader.GetAllAsDataTableAsync("file.dbf");
result.OnSuccess(data => Process(data))
      .OnFailure(error => Log(error));
```

## Reglas
1. Las entidades deben validar sus propias reglas intrinsecas
2. Los contratos (interfaces) definen QUE se necesita, no COMO se obtiene
3. No usar `async/await` en entidades (son objetos de datos puros)
4. **NO** usar tipos de UI (ObservableCollection, DataAnnotations, etc.)
5. **NO** depender de Data, Business, Presentation ni Common
6. Usar `List<T>` o `IEnumerable<T>` en lugar de colecciones de UI

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
public interface IDbfReader
{
    Task<Result<DataTable, string>> GetAllAsDataTableAsync(string filePath);
    int TotalImages { get; }
    int TotalClaims { get; }
}
```

## Aggregates (Domain-Driven Design)

### ¿Que son?
Un **Aggregate** es un grupo de objetos de dominio (entidades + value objects) tratados como una unidad para cambios de datos. Tiene una **raiz de agregado** (Aggregate Root) que es la unica entrada para modificar el agregado.

### ¿Cuando usar Aggregates?
✅ **USA Aggregates cuando:**
- Tienes entidades relacionadas que deben mantener **consistencia transaccional**
- Las reglas de negocio requieren validar **multiples entidades juntas**
- Necesitas **encapsular la complejidad** de relaciones entre entidades
- Implementas un **bounded context** con limites claros

❌ **NO uses Aggregates cuando:**
- Tienes entidades simples sin relaciones complejas
- No hay reglas de consistencia entre entidades
- Las entidades se pueden modificar independientemente

### Ejemplo de Aggregate
```csharp
// Domain/Aggregates/OrderAggregate/Order.cs (Aggregate Root)
public class Order
{
    public Guid Id { get; private set; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    
    // Solo la raiz puede agregar lineas (mantiene consistencia)
    public Result<bool, string> AddLine(OrderLine line)
    {
        if (_lines.Count >= 100)
            return Result<bool, string>.Fail("Maximo 100 lineas por orden");
        
        _lines.Add(line);
        return Result<bool, string>.Ok(true);
    }
    
    public decimal GetTotal() => _lines.Sum(l => l.Amount);
}

// Domain/Aggregates/OrderAggregate/OrderLine.cs (Entidad hija)
public class OrderLine
{
    public Guid Id { get; private set; }
    public string ProductCode { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    
    // Constructor privado: solo Order puede crear lineas
    internal OrderLine(string productCode, decimal amount)
    {
        Id = Guid.NewGuid();
        ProductCode = productCode;
        Amount = amount;
    }
}

// Domain/Aggregates/Abstractions/IOrderRepository.cs
public interface IOrderRepository
{
    // Opera sobre el agregado completo, no sobre lineas individuales
    Task<Result<Order, string>> GetByIdAsync(Guid orderId);
    Task<Result<Order, string>> SaveAsync(Order order);
}
```

### Reglas de Aggregates
1. **Una transaccion = Un agregado**: Cambios al agregado deben ser atomicos
2. **Referencias por ID**: Agregados se referencian entre si por ID, no por objeto
3. **Consistencia inmediata dentro, eventual entre agregados**
4. **Tamaño pequeño**: Agregados grandes causan problemas de rendimiento
