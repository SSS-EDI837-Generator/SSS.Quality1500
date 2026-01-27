# Diagramas de Flujo de Trabajo - SSS.Quality1500

Este documento contiene los diagramas de secuencia que ilustran el flujo de trabajo principal de la aplicación.

## Flujo Principal de la Aplicación

El siguiente diagrama muestra las 4 fases principales del workflow:
1. **Carga de Archivo** - Lectura de archivos DBF
2. **Validacion** - Ejecucion de reglas de validacion
3. **Revision de Errores** - Correccion interactiva con zoom
4. **Exportacion** - Generacion de reportes

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#4A90D9',
    'primaryTextColor': '#fff',
    'primaryBorderColor': '#2E5A8B',
    'lineColor': '#5A6C7D',
    'secondaryColor': '#F39C12',
    'tertiaryColor': '#E8F4FD'
  }
}}%%

sequenceDiagram
    autonumber

    box rgb(156,39,176) PRESENTATION LAYER
        participant U as Usuario
        participant V as View WPF
        participant VM as ViewModel
    end

    box rgb(33,150,243) BUSINESS LAYER
        participant PS as ClaimProcessingService
        participant VE as ValidationEngine
    end

    box rgb(76,175,80) DATA LAYER
        participant DR as DbfReader
        participant API as NpiApiClient
    end

    rect rgb(227,242,253)
        Note over U,API: FASE 1 - CARGA DE ARCHIVO DBF
        U->>V: Selecciona archivo .dbf
        V->>VM: LoadFileCommand(path)
        VM->>PS: ProcessFileAsync(path)
        PS->>DR: GetAllAsDataTableAsync(path)
        DR-->>PS: Result DataTable
        PS->>PS: MapToClaimRecords()
        PS-->>VM: Result List ClaimRecord
        VM-->>V: Actualiza ObservableCollection
        V-->>U: Muestra preview de registros
    end

    rect rgb(255,243,224)
        Note over U,API: FASE 2 - VALIDACION DE CLAIMS
        U->>V: Click Validar
        V->>VM: ValidateCommand()
        VM->>PS: ValidateClaimsAsync(claims)

        loop Para cada ClaimRecord
            PS->>VE: ValidateAsync(claim)

            rect rgb(255,235,204)
                Note over VE: Validaciones NPI
                VE->>VE: NpiFormatValidator
                VE->>VE: NpiChecksumValidator
                VE->>API: ValidateNpiAsync(npi)
                API-->>VE: Result NpiResponse
            end

            rect rgb(255,224,178)
                Note over VE: Validaciones de Fecha
                VE->>VE: DateFormatValidator
                VE->>VE: DateNotFutureValidator
                VE->>VE: DateRangeValidator
            end

            VE-->>PS: List ValidationError
        end

        PS-->>VM: Result ValidationSummary
        VM-->>V: Actualiza UI con errores
        V-->>U: Muestra resumen de validacion
    end

    rect rgb(232,245,233)
        Note over U,API: FASE 3 - REVISION DE ERRORES
        U->>V: Selecciona error a revisar
        V->>VM: SelectErrorCommand(error)
        VM->>VM: LoadClaimImage()
        VM->>VM: CalculateZoomArea(fieldName)
        VM-->>V: Actualiza FocusArea y ZoomLevel
        V-->>U: Muestra imagen con zoom en campo

        alt Usuario corrige campo
            rect rgb(200,230,201)
                U->>V: Edita valor del campo
                V->>VM: UpdateFieldCommand(field, value)
                VM->>VE: RevalidateFieldAsync(claim, field)
                VE-->>VM: Result bool
                VM->>DR: UpdateRecordAsync(claim)
                DR-->>VM: Result bool
                VM-->>V: Actualiza estado del error
                V-->>U: Marca error como corregido
            end
        else Usuario acepta error
            rect rgb(255,245,157)
                U->>V: Click Aceptar Error
                V->>VM: AcceptErrorCommand()
                VM->>DR: MarkErrorAcceptedAsync(errorId)
                DR-->>VM: Result bool
                VM-->>V: Siguiente error
            end
        end
    end

    rect rgb(243,229,245)
        Note over U,API: FASE 4 - EXPORTACION DE RESULTADOS
        U->>V: Click Exportar
        V->>VM: ExportCommand(format)

        alt Excel
            rect rgb(225,190,231)
                VM->>PS: ExportToExcelAsync(results)
                PS-->>VM: Result filepath
            end
        else PDF
            rect rgb(206,147,216)
                VM->>PS: ExportToPdfAsync(results)
                PS-->>VM: Result filepath
            end
        end

        VM-->>V: Muestra path del archivo
        V-->>U: Confirma exportacion exitosa
    end
```

---

## Flujo de Validación de Estructura DBF

Este diagrama muestra el flujo de validación de la estructura del archivo DBF antes de procesar los claims.
La validación verifica que el archivo contenga las 735 columnas esperadas (119 no-service-line + 28 lineas x 22 columnas).

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#4A90D9',
    'primaryTextColor': '#fff',
    'primaryBorderColor': '#2E5A8B',
    'lineColor': '#5A6C7D',
    'secondaryColor': '#F39C12',
    'tertiaryColor': '#E8F4FD'
  }
}}%%

sequenceDiagram
    autonumber

    box rgb(156,39,176) PRESENTATION LAYER
        participant U as Usuario
        participant V as ProcessingView
        participant VM as ProcessingViewModel
    end

    box rgb(33,150,243) BUSINESS LAYER
        participant DVS as DbfValidationService
        participant VC as VdeConstants
    end

    box rgb(76,175,80) DATA LAYER
        participant DR as DbfReader
    end

    rect rgb(227,242,253)
        Note over U,DR: FASE 0 - VALIDACION DE ESTRUCTURA DBF
        U->>V: Selecciona archivo .dbf
        V->>VM: ValidateDbfCommand(path)
        VM->>DVS: ValidateDbfFileAsync(path)
        DVS->>DR: GetAllAsDataTableAsync(path)
        DR-->>DVS: Result DataTable
    end

    rect rgb(255,243,224)
        Note over DVS,VC: VERIFICACION DE COLUMNAS
        DVS->>VC: GetAllExpectedColumns()
        VC-->>DVS: List 735 columnas esperadas
        DVS->>DVS: CompareActualVsExpected()
    end

    alt Estructura Valida (735 columnas presentes)
        rect rgb(200,230,201)
            DVS->>DVS: CalculateTotalClaims()
            DVS-->>VM: Result DbfValidationResult(IsValid=true)
            VM->>VM: TotalRecords = result.TotalRecords
            VM->>VM: TotalClaims = result.TotalClaims
            VM->>VM: CanProcess = true
            VM-->>V: Actualiza UI con totales
            V-->>U: Muestra totales y habilita Procesar
        end
    else Estructura Invalida (faltan columnas)
        rect rgb(255,205,210)
            DVS-->>VM: Result DbfValidationResult(IsValid=false)
            VM->>VM: CanProcess = false
            VM-->>V: Muestra error con columnas faltantes
            V-->>U: Deshabilita boton Procesar
        end
    end
```

### Componentes del Flujo

| Componente | Capa | Responsabilidad |
|------------|------|-----------------|
| `ProcessingViewModel` | Presentation | Coordina UI y comandos |
| `DbfValidationService` | Business | Valida estructura del DBF |
| `VdeConstants` | Domain | Define las 735 columnas esperadas |
| `DbfReader` | Data | Lee el archivo DBF |

### Resultado de Validación

```csharp
public class DbfValidationResult
{
    public bool IsValid { get; set; }           // True si todas las columnas existen
    public int TotalRecords { get; set; }       // Total de registros (imagenes)
    public int TotalClaims { get; set; }        // Claims (V1PAGINA != "99")
    public List<string> MissingColumns { get; set; }  // Columnas faltantes
    public string? ErrorMessage { get; set; }   // Mensaje de error
}
```

---

## Diagrama de Capas (Onion Architecture)

Este diagrama muestra la arquitectura Onion donde Business y Data son capas hermanas que ambas dependen de Domain:

```mermaid
%%{init: {
  'theme': 'base',
  'themeVariables': {
    'primaryColor': '#BB86FC',
    'lineColor': '#5A6C7D'
  }
}}%%

sequenceDiagram
    box rgb(156,39,176) PRESENTATION (Composition Root)
        participant P as Presentation
    end

    box rgb(33,150,243) BUSINESS
        participant B as Business
    end

    box rgb(76,175,80) DATA
        participant D as Data
    end

    box rgb(255,193,7) DOMAIN (Centro)
        participant Dom as Domain
    end

    box rgb(255,152,0) EXTERNAL
        participant E as External
    end

    Note over P,E: Onion Architecture - Flujo de Dependencias

    rect rgb(243,229,245)
        Note over P: Composition Root: registra Business + Data
        P->>B: Request via CQRS Handler
        P->>D: Registra implementaciones de Domain
    end

    rect rgb(227,242,253)
        Note over B,Dom: Business usa interfaces de Domain
        B->>Dom: Usa IDbfReader (contrato)
    end

    rect rgb(232,245,233)
        Note over D,Dom: Data implementa interfaces de Domain
        D->>Dom: Implementa IDbfReader
        D->>E: I/O Operation
        E-->>D: Raw Data
    end

    rect rgb(232,245,233)
        D-->>B: Result Entity o Error (via DI)
    end

    rect rgb(227,242,253)
        B-->>P: Result DTO o Error
    end

    Note over P: UI actualiza con ObservableCollection
    Note over B: Orquesta con interfaces de Domain (NO Data directo)
    Note over D: Implementa contratos definidos en Domain
    Note over Dom: Define interfaces y Result<T,E>
    Note over E: Archivos DBF - NPI API
```

---

## Flujo del Result Pattern

Este diagrama ilustra como se utiliza el patron `Result<T,E>` para manejar operaciones que pueden fallar:

```mermaid
%%{init: {'theme': 'base'}}%%

sequenceDiagram
    participant C as Caller
    participant S as Service
    participant R as Result

    rect rgb(240,240,240)
        Note over C,R: RESULT PATTERN FLOW
        C->>S: GetDataAsync()
    end

    alt Operacion exitosa
        rect rgb(200,230,201)
            S->>R: Result.Ok(data)
            R-->>C: Success con Value = data
            C->>C: result.OnSuccess(Process)
            Note over C: Continua flujo normal
        end
    else Operacion fallida
        rect rgb(255,205,210)
            S->>R: Result.Fail(error)
            R-->>C: Failure con Error = error
            C->>C: result.OnFailure(LogError)
            Note over C: Maneja el error
        end
    end

    rect rgb(227,242,253)
        Note over C,R: PATTERN MATCHING
        C->>C: result.Match(onSuccess, onFailure)
    end
```

---

## Leyenda de Colores

| Color | Capa/Fase | Codigo RGB |
|-------|-----------|------------|
| Purpura | Presentation Layer | `rgb(156,39,176)` |
| Azul | Business Layer | `rgb(33,150,243)` |
| Verde | Data Layer | `rgb(76,175,80)` |
| Naranja | External Systems | `rgb(255,152,0)` |
| Azul claro | Fase 1: Carga | `rgb(227,242,253)` |
| Naranja claro | Fase 2: Validacion | `rgb(255,243,224)` |
| Verde claro | Fase 3: Revision | `rgb(232,245,233)` |
| Purpura claro | Fase 4: Exportacion | `rgb(243,229,245)` |
| Verde pastel | Exito / Correccion | `rgb(200,230,201)` |
| Amarillo pastel | Aceptar Error | `rgb(255,245,157)` |
| Rojo pastel | Error / Fallo | `rgb(255,205,210)` |
| Purpura medio | Export Excel | `rgb(225,190,231)` |
| Purpura oscuro | Export PDF | `rgb(206,147,216)` |

---

## Componentes por Capa (Onion Architecture)

### Domain Layer (Centro - Sin dependencias)
- **Entities**: ClaimRecord, PatientInfo, ProviderInfo
- **Value Objects**: Objetos inmutables de dominio
- **Interfaces**: Contratos (IDbfReader, INpiApiClient) implementados por Data
- **Result<T,E>**: Patron para manejo de errores
- **CQRS**: IQueryHandler, ICommandHandler

### Common Layer (Sin dependencias)
- **Utilities**: EnvironmentProvider, VersionInfo
- **Services**: LazyService

### Business Layer (Depende solo de Domain)
- **CQRS Handlers**: ValidateDbfHandler, ProcessClaimsHandler
- **Services**: ValidationEngine (usa interfaces de Domain)
- **Validators**: NpiFormatValidator, DateRangeValidator
- **DTOs**: Objetos de transferencia

### Data Layer (Depende de Domain y Common)
- **Services**: DbfReader (implementa IDbfReader de Domain)
- **ApiClients**: NpiApiClient (implementa INpiApiClient de Domain)
- **Repositories**: Implementaciones de repositorios

### Presentation Layer (Composition Root - Business y Data)
- **Views**: WPF XAML con MaterialDesignThemes
- **ViewModels**: CommunityToolkit.Mvvm con `[ObservableProperty]` y `[RelayCommand]`
- **Services**: NavigationService, DialogService
- **DI Registration**: Registra servicios de Business Y Data

---

## Persistencia de Datos

Esta solucion utiliza **archivos DBF** como unico mecanismo de persistencia:

- **Lectura**: `DbfReader.GetAllAsDataTableAsync(path)`
- **Escritura**: `DbfReader.UpdateRecordAsync(claim)`
- **No se usa**: SQLite, EF Core, ni base de datos local

Los archivos DBF son el formato nativo de los datos de claims CMS-1500.

---

## Referencias

- [PLAN_TRABAJO.md](../PLAN_TRABAJO.md) - Plan de trabajo detallado
- [CLAUDE.md](../CLAUDE.md) - Guia de arquitectura
- [HEALTHCARE_CONTEXT.md](./HEALTHCARE_CONTEXT.md) - Contexto de CMS-1500
- [VALIDATION_RULES.md](./VALIDATION_RULES.md) - Reglas de validacion
