# Plan de Trabajo: Procesamiento y Vista de Errores

**Fecha inicio**: 2026-01-27
**Última actualización**: 2026-01-27
**Estado**: En Progreso (Día 1 completado)

---

## Resumen Ejecutivo

Implementar el botón "Procesar Archivo" con validaciones reales y crear la vista "Ver Errores" para revisar y corregir registros con errores.

---

## Parte 1: Procesar Archivo

### 1.1 Configuración de APIs (appsettings.json)

Los endpoints ya existen en configuración:
- `SssMembersApiEndpoint` - Validar subscriber/member
- `SssB2BApiEndpoint` - Validar NPI del proveedor

**Tarea**: Crear clases de configuración para inyectar estos valores.

```
Business/Configuration/
├── ApiSettings.cs          # Clase con URLs de APIs
└── ProcessingSettings.cs   # Configuración general de procesamiento
```

### 1.2 Servicios de Validación (Business Layer)

```
Business/Services/
├── Interfaces/
│   ├── INpiValidationService.cs
│   ├── IMemberValidationService.cs
│   ├── IDateValidationService.cs
│   ├── IIcd10ValidationService.cs
│   └── IClaimProcessingService.cs    # Orquestador principal
│
├── Validation/
│   ├── NpiValidationService.cs       # Cliente API → NPPES fallback
│   ├── MemberValidationService.cs    # Cliente API
│   ├── DateValidationService.cs      # Validación local
│   └── Icd10ValidationService.cs     # Archivo local
│
└── Processing/
    └── ClaimProcessingService.cs     # Orquesta todas las validaciones
```

### 1.3 Flujo de Validación NPI

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  Leer NPI del   │ ──► │  Buscar en API  │ ──► │  ¿Encontrado?   │
│     DBF         │     │    Cliente      │     │                 │
└─────────────────┘     └─────────────────┘     └────────┬────────┘
                                                         │
                                          ┌──────────────┴──────────────┐
                                          │                             │
                                         SÍ                            NO
                                          │                             │
                                          ▼                             ▼
                                   ┌─────────────┐              ┌─────────────┐
                                   │   Válido    │              │ Buscar en   │
                                   │             │              │   NPPES     │
                                   └─────────────┘              └──────┬──────┘
                                                                       │
                                                         ┌─────────────┴─────────────┐
                                                         │                           │
                                                        SÍ                          NO
                                                         │                           │
                                                         ▼                           ▼
                                                  ┌─────────────┐            ┌─────────────┐
                                                  │   Válido    │            │   ERROR     │
                                                  └─────────────┘            └─────────────┘
```

### 1.4 Validaciones por Tipo de Campo

| Tipo de Campo | Servicio | Fuente de Validación |
|---------------|----------|---------------------|
| NPI Proveedor | NpiValidationService | API Cliente → NPPES |
| NPI Facility | NpiValidationService | API Cliente → NPPES |
| Subscriber ID | MemberValidationService | API Cliente |
| Fechas | DateValidationService | Local (formato + no futuras) |
| ICD-10 | Icd10ValidationService | Archivo local JSON |
| CPT/HCPCS | (Futuro) | Archivo local JSON |

### 1.5 Archivo Local ICD-10

Descargar códigos ICD-10-CM de CMS y crear archivo JSON:

```
Data/Resources/
└── icd10-codes.json
```

Formato:
```json
{
  "version": "2024",
  "codes": {
    "A00.0": "Cholera due to Vibrio cholerae 01, biovar cholerae",
    "A00.1": "Cholera due to Vibrio cholerae 01, biovar eltor",
    ...
  }
}
```

### 1.6 Modelo de Resultados de Procesamiento

```csharp
// Domain/Models/ProcessingResult.cs
public sealed class ProcessingResult
{
    public int TotalRecords { get; init; }
    public int RecordsWithErrors { get; init; }
    public int RecordsValid { get; init; }
    public List<RecordValidationResult> Errors { get; init; } = [];
    public DateTime ProcessedAt { get; init; }
}

// Domain/Models/RecordValidationResult.cs
public sealed class RecordValidationResult
{
    public int RecordIndex { get; init; }
    public string ImageFileName { get; init; } = string.Empty;
    public Dictionary<string, object?> RecordData { get; init; } = [];
    public List<FieldValidationError> FieldErrors { get; init; } = [];
}

// Domain/Models/FieldValidationError.cs
public sealed class FieldValidationError
{
    public string ColumnName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public object? CurrentValue { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public ValidationErrorType ErrorType { get; init; }
}

// Domain/Enums/ValidationErrorType.cs
public enum ValidationErrorType
{
    InvalidFormat,
    NotFound,
    FutureDate,
    InvalidCode,
    Required,
    OutOfRange
}
```

---

## Parte 2: Vista de Errores

### 2.1 Nueva Vista y ViewModel

```
Presentation/
├── Views/
│   └── ErrorReviewUserControl.xaml       # Nueva vista
│
├── ViewModels/
│   └── ErrorReviewViewModel.cs           # Nuevo ViewModel
│
└── Models/
    └── ErrorRecordViewModel.cs           # Modelo para binding
```

### 2.2 Diseño de la Vista de Errores

```
┌─────────────────────────────────────────────────────────────────────┐
│  Revisión de Errores                                                │
│  Registro 3 de 14 con errores                                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                                                             │   │
│  │                    [IMAGEN TIF]                             │   │
│  │                                                             │   │
│  │              (Visor de imagen con zoom)                     │   │
│  │                                                             │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│  Errores en este registro (3):                                      │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ ⚠ NPI Proveedor (V0NPIPROV): 1234567890 - NPI no encontrado │   │
│  │ ⚠ Fecha Servicio (V0FECSER): 2027-01-15 - Fecha futura      │   │
│  │ ⚠ Diagnóstico 1 (V0ICD101): Z99.99 - Código ICD-10 inválido │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│  Datos del Registro:                                                │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │  Campo              Valor                      Editable     │   │
│  │  ────────────────────────────────────────────────────────   │   │
│  │  V0NPIPROV          [1234567890        ]       ✓ (config)   │   │
│  │  V0FECSER           [2027-01-15        ]       ✓ (config)   │   │
│  │  V0ICD101           [Z99.99            ]       ✓ (config)   │   │
│  │  V0NOMBRE           Juan Pérez                 ✗ (solo ver) │   │
│  │  V0IFNAME01         19703701.tif               ✗ (solo ver) │   │
│  │  ...                                                        │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                     │
├─────────────────────────────────────────────────────────────────────┤
│  [← Anterior]  [Guardar Cambios]  [Revalidar]  [Siguiente →]       │
└─────────────────────────────────────────────────────────────────────┘
```

### 2.3 Funcionalidades de la Vista

| Botón | Acción |
|-------|--------|
| **← Anterior** | Navegar al registro anterior con errores |
| **Siguiente →** | Navegar al siguiente registro con errores |
| **Guardar Cambios** | Guardar cambios al DBF |
| **Revalidar** | Re-ejecutar validaciones en el registro actual |

### 2.4 Visor de Imágenes TIF

- Mostrar imagen TIF desde la ruta seleccionada + nombre en columna `V0IFNAME01`
- Controles de zoom (+ / - / ajustar)
- Posibilidad de rotar imagen si es necesario

---

## Parte 3: Arquitectura de Componentes

### 3.1 Diagrama de Dependencias

```
┌─────────────────────────────────────────────────────────────────┐
│                      PRESENTATION                                │
│  ┌─────────────────┐    ┌─────────────────┐                     │
│  │ ProcessingVM    │───►│ ErrorReviewVM   │                     │
│  └────────┬────────┘    └────────┬────────┘                     │
│           │                      │                               │
└───────────┼──────────────────────┼───────────────────────────────┘
            │                      │
            ▼                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                        BUSINESS                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │              ClaimProcessingService                      │    │
│  │  (Orquesta validaciones, usa configuración de columnas)  │    │
│  └──────┬─────────┬─────────┬─────────┬────────────────────┘    │
│         │         │         │         │                          │
│         ▼         ▼         ▼         ▼                          │
│  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐            │
│  │   NPI    │ │  Member  │ │   Date   │ │  ICD-10  │            │
│  │ Service  │ │ Service  │ │ Service  │ │ Service  │            │
│  └──────────┘ └──────────┘ └──────────┘ └──────────┘            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
            │                      │
            ▼                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                          DATA                                    │
│  ┌─────────────────┐    ┌─────────────────┐                     │
│  │   DbfReader     │    │  HttpClients    │                     │
│  │   DbfWriter     │    │  (APIs)         │                     │
│  └─────────────────┘    └─────────────────┘                     │
│                                                                  │
│  ┌─────────────────┐    ┌─────────────────┐                     │
│  │ColumnConfigRepo │    │ Icd10Repository │                     │
│  └─────────────────┘    └─────────────────┘                     │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Nuevas Interfaces en Domain

```csharp
// Domain/Interfaces/IDbfWriter.cs
public interface IDbfWriter
{
    Result<bool, string> UpdateRecord(string filePath, int recordIndex, Dictionary<string, object?> values);
}

// Domain/Interfaces/INppesApiClient.cs
public interface INppesApiClient
{
    Task<Result<NpiInfo?, string>> ValidateNpiAsync(string npi);
}

// Domain/Interfaces/IIcd10Repository.cs
public interface IIcd10Repository
{
    bool IsValidCode(string code);
    string? GetDescription(string code);
}
```

---

## Parte 4: Orden de Implementación

### Día 1 ✅ COMPLETADO (2026-01-27)

| # | Tarea | Capa | Estado |
|---|-------|------|--------|
| 1 | Crear modelos de resultado (ClaimProcessingResult, FieldValidationError, etc.) | Domain | ✅ `Domain/Models/` |
| 2 | Crear interfaz IDbfWriter y implementación | Domain/Data | ✅ `IDbfWriter.cs`, `DbfWriter.cs` |
| 3 | Crear DateValidationService (validación local) | Business | ✅ CQRS: `ValidateDateHandler` |
| 4 | Crear Icd10Repository con archivo JSON de códigos | Data | ✅ `Icd10Repository.cs` + JSON |
| 5 | Crear Icd10ValidationService | Business | ✅ CQRS: `ValidateIcd10Handler` |
| 6 | Crear estructura de ClaimProcessingService (orquestador) | Business | ✅ CQRS: `ProcessClaimsHandler` |

**Nota**: Se usó patrón CQRS en lugar de servicios tradicionales para validaciones.

### Día 2

| # | Tarea | Capa | Prioridad |
|---|-------|------|-----------|
| 7 | Integrar APIs del cliente (NPI, Member) - con info proporcionada | Data | Alta |
| 8 | Crear NpiValidationService con fallback a NPPES | Business | Alta |
| 9 | Crear MemberValidationService | Business | Alta |
| 10 | Conectar ProcessingViewModel con ClaimProcessingService | Presentation | Alta |

### Día 3

| # | Tarea | Capa | Prioridad |
|---|-------|------|-----------|
| 11 | Crear ErrorReviewUserControl.xaml (diseño) | Presentation | Alta |
| 12 | Crear ErrorReviewViewModel | Presentation | Alta |
| 13 | Implementar visor de imágenes TIF | Presentation | Media |
| 14 | Implementar navegación entre registros con errores | Presentation | Alta |

### Día 4

| # | Tarea | Capa | Prioridad |
|---|-------|------|-----------|
| 15 | Implementar edición de campos | Presentation | Alta |
| 16 | Implementar guardado de cambios al DBF | Data | Alta |
| 17 | Implementar revalidación de registro | Business | Media |
| 18 | Pruebas de integración completas | Todos | Alta |

---

## Parte 5: Datos Necesarios del Usuario

Antes de comenzar, necesito:

- [ ] Documentación/ejemplos de `SssMembersApiEndpoint` (request/response)
- [ ] Documentación/ejemplos de `SssB2BApiEndpoint` (request/response)
- [ ] Método de autenticación de las APIs (API key, header, etc.)
- [ ] Confirmar columnas de NPI en el DBF (¿V0NPIPROV? ¿otros?)
- [ ] Confirmar columnas de fechas a validar
- [ ] Confirmar columnas de ICD-10 a validar

---

## Notas Técnicas

### Escritura de DBF ✅ IMPLEMENTADO

Se implementó `DbfWriter` usando manipulación directa de bytes:
- Sin dependencias adicionales (compatible con .NET 10)
- Métodos: `UpdateRecord`, `UpdateRecords`, `MarkRecordDeleted`
- Soporta campos: Character, Numeric, Float, Date, Logical
- Encoding: CP1252 (estándar DBF)

**Archivos**:
- `Domain/Interfaces/IDbfWriter.cs`
- `Data/Services/DbfWriter.cs`

### Visor de Imágenes TIF

WPF soporta TIF nativo con `BitmapImage`. Para funcionalidades avanzadas:
- Zoom: `ScaleTransform` en un `ScrollViewer`
- Multi-página TIF: `BitmapDecoder.Frames`

### Cache de Validaciones

Considerar cache para evitar llamadas repetidas a APIs:
- Cache de NPIs validados (en memoria, durante sesión)
- Cache de Members validados (en memoria, durante sesión)

---

## Checklist Final

- [x] Build sin warnings
- [ ] Tests unitarios para servicios de validación
- [ ] Manejo de errores de red (APIs no disponibles)
- [ ] Logging de operaciones
- [x] UI responsive durante procesamiento (async/await)
- [x] Snackbar para notificaciones de éxito/error

---

## Próximos Pasos (Para Continuar)

### Opción A: Vista de Errores (Día 3) - RECOMENDADO
Ya tenemos `LastProcessingResult` en `ProcessingViewModel` con los errores.
Podemos crear la vista de revisión sin necesidad de las APIs.

**Tareas**:
1. Crear `ErrorReviewUserControl.xaml` con diseño (ver sección 2.2)
2. Crear `ErrorReviewViewModel.cs` con navegación
3. Implementar visor de imágenes TIF (WPF nativo)
4. Conectar navegación desde `ProcessingViewModel.ViewErrors()`

**Archivos clave a crear**:
```
Presentation/Views/ErrorReviewUserControl.xaml
Presentation/ViewModels/ErrorReviewViewModel.cs
```

**Datos disponibles** (de `ClaimProcessingResult`):
- `ErrorRecords` - Lista de registros con errores
- Cada registro tiene: `RecordIndex`, `ImageFileName`, `RecordData`, `FieldErrors`

### Opción B: APIs NPI/Member (Día 2)
Requiere documentación de APIs del cliente antes de implementar.

**Pendiente del usuario**:
- [ ] Documentación de `SssMembersApiEndpoint`
- [ ] Documentación de `SssB2BApiEndpoint`
- [ ] Método de autenticación
