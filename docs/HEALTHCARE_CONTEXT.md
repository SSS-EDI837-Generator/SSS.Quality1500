# HEALTHCARE_CONTEXT.md

## Contexto de Negocio - CMS-1500 Forms

Este documento proporciona contexto específico de healthcare para SSS.Quality1500, complementando la arquitectura definida en CLAUDE.md.

## ¿Qué es el formulario CMS-1500?

El formulario CMS-1500 (Centers for Medicare & Medicaid Services) es el formulario estándar utilizado por profesionales de salud para facturar a:
- Medicare Part B
- Medicaid  
- Aseguradoras privadas
- Otros programas de salud

Es el equivalente profesional del UB-04 (usado por hospitales/instituciones).

## Estructura del Formulario CMS-1500

### Secciones Principales

#### 1. Información del Carrier (Boxes 1-13)
Información del seguro y paciente.

#### 2. Información del Paciente y Asegurado (Boxes 1a-13)
- Box 1: Tipo de seguro (Medicare, Medicaid, Tricare, etc.)
- Box 2: Patient Name (Last, First, Middle)
- Box 3: Patient Date of Birth + Gender
- Box 5: Patient Address
- Box 9: Other Insured's Name (si aplica)

#### 3. Información del Physician/Supplier (Boxes 14-33)
- Box 17: Referring Provider Name
- Box 17a: Other ID del referring provider
- Box 17b: **NPI del Referring Provider** ⭐
- Box 24J: **Rendering Provider NPI** ⭐
- Box 33: **Billing Provider Info y NPI** ⭐

#### 4. Información de Servicio (Boxes 24a-24j)
Líneas de servicio (hasta 6 en formulario físico):
- Box 24A: Dates of Service (FROM - TO)
- Box 24B: Place of Service Code
- Box 24D: Procedures, Services, Supplies (CPT/HCPCS codes)
- Box 24E: Diagnosis Pointer (referencia a Box 21)
- Box 24F: Charges
- Box 24J: Rendering Provider ID (NPI)

#### 5. Información de Diagnóstico (Box 21)
Códigos ICD-10 (hasta 12 códigos):
- A-L: Diagnosis codes
- Usa letras para referenciar desde líneas de servicio

## Campos Críticos para Validación

### NPIs (National Provider Identifier)
**Formato:** 10 dígitos numéricos  
**Ubicaciones en formulario:**
- Box 17b: Referring Provider NPI
- Box 24J: Rendering Provider NPI (por línea de servicio)
- Box 33a: Billing Provider NPI

**Reglas de validación:**
1. Debe ser numérico de exactamente 10 dígitos
2. Debe pasar validación de checksum (algoritmo Luhn)
3. Debe existir en NPPES (National Plan and Provider Enumeration System)
4. No debe estar en lista de exclusión OIG/LEIE

### Fechas de Servicio
**Box 24A:** Date of Service FROM - TO

**Reglas:**
- Formato: MM/DD/YYYY
- FROM no puede ser futura
- TO debe ser >= FROM
- Servicio debe ser después de DOB del paciente
- Considerar timely filing limits (típicamente 365 días)

### Códigos de Procedimiento (CPT/HCPCS)
**Box 24D:** Procedure codes

**Formato:**
- CPT: 5 dígitos numéricos (ej: 99213)
- HCPCS: 1 letra + 4 dígitos (ej: J3490)

**Modificadores:** Box 24D puede incluir hasta 4 modificadores de 2 caracteres

### Códigos de Diagnóstico (ICD-10)
**Box 21:** ICD-10 Diagnosis Codes

**Formato:** 
- 3-7 caracteres alfanuméricos
- Letra inicial + números
- Puede incluir punto decimal (ej: E11.9)

**Reglas:**
- Al menos 1 código requerido (A)
- Máximo 12 códigos (A-L)
- Diagnosis Pointer en Box 24E debe referenciar códigos válidos

### Place of Service
**Box 24B:** Place of Service Code

**Códigos comunes:**
| Código | Descripción |
|--------|-------------|
| 11 | Office |
| 12 | Home |
| 21 | Inpatient Hospital |
| 22 | Outpatient Hospital |
| 23 | Emergency Room - Hospital |
| 24 | Ambulatory Surgical Center |
| 31 | Skilled Nursing Facility |

## Mapeo a Base de Datos VDE (.dbf)

### Estructura Típica de Campos DBF

Los sistemas legacy de healthcare típicamente usan Visual FoxPro con estructura similar a:

```
Campo DBF             Tipo   Long  Descripción                    Box CMS-1500
--------------------------------------------------------------------------------
CLAIM_ID              C      20    ID único del claim             (interno)
BATCH_ID              C      20    ID del batch                   (interno)
PATIENT_FNAME         C      35    Patient First Name             Box 2
PATIENT_LNAME         C      35    Patient Last Name              Box 2
PATIENT_MI            C      1     Patient Middle Initial         Box 2
PATIENT_DOB           D      8     Patient Date of Birth          Box 3
PATIENT_GENDER        C      1     Patient Gender (M/F/U)         Box 3
PATIENT_ADDR1         C      50    Patient Address Line 1         Box 5
PATIENT_CITY          C      30    Patient City                   Box 5
PATIENT_STATE         C      2     Patient State                  Box 5
PATIENT_ZIP           C      10    Patient ZIP                    Box 5

DOS_FROM              D      8     Date of Service FROM           Box 24A
DOS_TO                D      8     Date of Service TO             Box 24A
PLACE_SERVICE         C      2     Place of Service Code          Box 24B
PROCEDURE_CODE        C      5     CPT/HCPCS Code                 Box 24D
MODIFIER_1            C      2     Modifier 1                     Box 24D
MODIFIER_2            C      2     Modifier 2                     Box 24D
MODIFIER_3            C      2     Modifier 3                     Box 24D
MODIFIER_4            C      2     Modifier 4                     Box 24D
DIAG_POINTER          C      4     Diagnosis Pointer (ej: "AB")   Box 24E
CHARGE_AMOUNT         N      10,2  Charge Amount                  Box 24F

RENDERING_NPI         C      10    Rendering Provider NPI         Box 24J
REFERRING_NPI         C      10    Referring Provider NPI         Box 17b
BILLING_NPI           C      10    Billing Provider NPI           Box 33a

DIAGNOSIS_1           C      7     ICD-10 Code A                  Box 21A
DIAGNOSIS_2           C      7     ICD-10 Code B                  Box 21B
DIAGNOSIS_3           C      7     ICD-10 Code C                  Box 21C
DIAGNOSIS_4           C      7     ICD-10 Code D                  Box 21D
DIAGNOSIS_5           C      7     ICD-10 Code E                  Box 21E
DIAGNOSIS_6           C      7     ICD-10 Code F                  Box 21F
DIAGNOSIS_7           C      7     ICD-10 Code G                  Box 21G
DIAGNOSIS_8           C      7     ICD-10 Code H                  Box 21H
DIAGNOSIS_9           C      7     ICD-10 Code I                  Box 21I
DIAGNOSIS_10          C      7     ICD-10 Code J                  Box 21J
DIAGNOSIS_11          C      7     ICD-10 Code K                  Box 21K
DIAGNOSIS_12          C      7     ICD-10 Code L                  Box 21L

IMAGE_PATH            C      255   Path to scanned form image     (interno)
REVIEW_STATUS         C      20    Status de revisión             (interno)
REVIEWED_BY           C      50    Usuario que revisó             (interno)
REVIEWED_DATE         D      8     Fecha de revisión              (interno)
ERROR_FLAGS           M            Memo con errores encontrados   (interno)
```

## Entidades de Domain Recomendadas

Basado en Clean Architecture, estas son las entidades que deberían existir en `Domain/Models/`:

### 1. ClaimRecord (Entidad principal)
```csharp
namespace SSS.Quality1500.Domain.Models;

public sealed class ClaimRecord
{
    public string ClaimId { get; init; } = string.Empty;
    public string BatchId { get; init; } = string.Empty;
    
    // Patient Info
    public PatientInfo Patient { get; init; } = new();
    
    // Service Info
    public ServiceInfo Service { get; init; } = new();
    
    // Provider Info
    public ProviderInfo Provider { get; init; } = new();
    
    // Diagnosis
    public List<string> DiagnosisCodes { get; init; } = new();
    
    // Image reference
    public string ImagePath { get; init; } = string.Empty;
    
    // Review status
    public ReviewStatus Status { get; init; } = ReviewStatus.Pending;
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(ClaimId)
            && Patient.IsValid()
            && Service.IsValid()
            && Provider.IsValid()
            && DiagnosisCodes.Any();
    }
}
```

### 2. PatientInfo (Value Object)
```csharp
public sealed record PatientInfo
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string MiddleInitial { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public Gender Gender { get; init; }
    public Address Address { get; init; } = new();
    
    public string FullName => $"{LastName}, {FirstName} {MiddleInitial}".Trim();
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FirstName)
            && !string.IsNullOrWhiteSpace(LastName)
            && DateOfBirth != default;
    }
}
```

### 3. ServiceInfo (Value Object)
```csharp
public sealed record ServiceInfo
{
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public string PlaceOfServiceCode { get; init; } = string.Empty;
    public string ProcedureCode { get; init; } = string.Empty;
    public List<string> Modifiers { get; init; } = new();
    public string DiagnosisPointer { get; init; } = string.Empty;
    public decimal ChargeAmount { get; init; }
    
    public bool IsValid()
    {
        return DateFrom != default
            && DateTo >= DateFrom
            && !string.IsNullOrWhiteSpace(ProcedureCode)
            && ChargeAmount > 0;
    }
}
```

### 4. ProviderInfo (Value Object)
```csharp
public sealed record ProviderInfo
{
    public string RenderingNpi { get; init; } = string.Empty;
    public string ReferringNpi { get; init; } = string.Empty;
    public string BillingNpi { get; init; } = string.Empty;
    
    public bool IsValid()
    {
        // Al menos uno de los NPIs debe estar presente
        return !string.IsNullOrWhiteSpace(RenderingNpi)
            || !string.IsNullOrWhiteSpace(ReferringNpi)
            || !string.IsNullOrWhiteSpace(BillingNpi);
    }
}
```

### 5. ValidationError (Entidad)
```csharp
public sealed class ValidationError
{
    public string FieldName { get; init; } = string.Empty;
    public string ErrorCode { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public ErrorSeverity Severity { get; init; }
    public string CurrentValue { get; init; } = string.Empty;
    public string ExpectedValue { get; init; } = string.Empty;
}
```

## Enums de Domain Recomendados

En `Domain/Enums/`:

```csharp
public enum ReviewStatus
{
    Pending,
    Validating,
    HasErrors,
    HasWarnings,
    UnderReview,
    Reviewed,
    Corrected,
    Accepted,
    Rejected
}

public enum ErrorSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public enum Gender
{
    Male,
    Female,
    Unknown
}

public enum InsuranceType
{
    Medicare,
    Medicaid,
    Tricare,
    Champva,
    GroupHealthPlan,
    FecaBlkLung,
    Other
}
```

## Compliance y Seguridad

### HIPAA Compliance

Los datos del formulario CMS-1500 contienen **PHI (Protected Health Information)**:
- Nombres de pacientes
- Fechas de nacimiento
- Direcciones
- Diagnósticos
- Información de tratamiento

### Requisitos Técnicos

1. **Logging sin PHI:**
```csharp
// ❌ MALO - loguea PHI
_logger.LogInformation("Processing claim for patient {PatientName}", claim.Patient.FullName);

// ✅ BUENO - solo IDs
_logger.LogInformation("Processing claim {ClaimId} in batch {BatchId}", claim.ClaimId, claim.BatchId);
```

2. **Encriptación:**
- Datos en reposo: Si se almacenan en SQLite, considerar encriptación
- Datos en tránsito: API calls deben usar HTTPS/TLS
- Credenciales: Usar configuración externa, nunca hardcodear

3. **Audit Trail:**
- Registrar quién accedió a cada claim
- Registrar cambios/correcciones realizadas
- Mantener historial de validaciones

4. **Sesiones:**
- Timeout automático después de inactividad
- No almacenar PHI en caché del navegador
- Limpiar memoria al cerrar sesión

## Recursos Externos

### Documentación Oficial
- [CMS-1500 Form Instructions](https://www.cms.gov/medicare/cms-forms/cms-forms/downloads/cms1500.pdf)
- [NPI Registry](https://npiregistry.cms.hhs.gov/)
- [ICD-10 Codes](https://www.icd10data.com/)
- [CPT Codes](https://www.ama-assn.org/practice-management/cpt)
- [Place of Service Codes](https://www.cms.gov/medicare/coding-billing/place-of-service-codes/code-place-of-service)

### Listas de Validación
- [OIG Exclusion List (LEIE)](https://oig.hhs.gov/exclusions/)
- [NPPES NPI Registry](https://npiregistry.cms.hhs.gov/)
