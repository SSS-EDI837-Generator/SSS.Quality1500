# DBF Schema - SSS1503.DBF

Este documento define la estructura oficial del archivo DBF utilizado para los claims CMS-1500.
Total: **735 columnas**.

## Resumen de Estructura

| Categoria | Columnas | Rango |
|-----------|----------|-------|
| Non-Service-Line | 119 | Fields 1-119 |
| Service Lines (28 x 22) | 616 | Fields 120-735 |
| **TOTAL** | **735** | |

---

## Columnas Non-Service-Line (1-119)

### Document Metadata (V0*) - 11 columnas

| # | Field Name | Type | Length | Descripcion |
|---|------------|------|--------|-------------|
| 1 | V0DOCUMENT | Character | 22 | Document identifier |
| 2 | V0BATCHNUM | Character | 8 | Batch number |
| 3 | V0SEQUENCE | Character | 8 | Sequence number |
| 4 | V0CURSTAGE | Character | 2 | Current stage |
| 5 | V0EXPRUNID | Character | 8 | Export run ID |
| 6 | V0KEYOPER | Character | 4 | Key operator |
| 7 | V0VFYOPER | Character | 4 | Verify operator |
| 8 | V0VIEWNAME | Character | 8 | View name |
| 9 | V0FILEPATH | Character | 64 | File path |
| 10 | V0IFNAME01 | Character | 36 | Image file name |
| 11 | V0CONFIDNC | Memo | 10 | Confidence |

### Page (V1*) - 1 columna

| # | Field Name | Type | Length | Descripcion |
|---|------------|------|--------|-------------|
| 12 | V1PAGINA | Character | 2 | Page number (99 = continuation) |

### Patient Information - Box 1-4 (V1*) - 8 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 13 | V11TYPEID | Character | 1 | 1 | Insurance type |
| 14 | V11AZUA | Character | 3 | 1a | ZUA number |
| 15 | V11AINSURE | Character | 15 | 1a | Insured's ID number |
| 16 | V12LASTNAM | Character | 30 | 2 | Patient's last name |
| 17 | V12NAME | Character | 15 | 2 | Patient's first name |
| 18 | V12INITIAL | Character | 1 | 2 | Patient's middle initial |
| 19 | V13BIRTH | Character | 8 | 3 | Patient's birth date |
| 20 | V13SEXO | Character | 1 | 3 | Patient's sex |

### Insured Information - Box 4 (V1*) - 3 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 21 | V14LASTNAM | Character | 30 | 4 | Insured's last name |
| 22 | V14NAME | Character | 15 | 4 | Insured's first name |
| 23 | V14INITIAL | Character | 1 | 4 | Insured's middle initial |

### Patient Address - Box 5 (V1*) - 7 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 24 | V15ADDRES1 | Character | 25 | 5 | Address line 1 |
| 25 | V15ADDRES2 | Character | 25 | 5 | Address line 2 |
| 26 | V15CITY | Character | 20 | 5 | City |
| 27 | V15STATE | Character | 3 | 5 | State |
| 28 | V15ZIPCODE | Character | 5 | 5 | ZIP code |
| 29 | V15PLUS4 | Character | 4 | 5 | ZIP+4 |
| 30 | V15TELEFON | Character | 10 | 5 | Telephone |

### Patient Relationship - Box 6 (V1*) - 1 columna

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 31 | V16PATRELA | Character | 1 | 6 | Patient relationship to insured |

### Insured Address - Box 7 (V1*) - 7 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 32 | V17ADDRES1 | Character | 25 | 7 | Address line 1 |
| 33 | V17ADDRES2 | Character | 25 | 7 | Address line 2 |
| 34 | V17CITY | Character | 20 | 7 | City |
| 35 | V17STATE | Character | 3 | 7 | State |
| 36 | V17ZIPCODE | Character | 5 | 7 | ZIP code |
| 37 | V17PLUS4 | Character | 4 | 7 | ZIP+4 |
| 38 | V17TELEFON | Character | 10 | 7 | Telephone |

### Reserved and Other Insured - Box 8-9 (V2*) - 8 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 39 | V28RESERVE | Character | 10 | 8 | Reserved |
| 40 | V29LASTNAM | Character | 30 | 9 | Other insured's last name |
| 41 | V29NAME | Character | 15 | 9 | Other insured's first name |
| 42 | V29INITIAL | Character | 1 | 9 | Other insured's middle initial |
| 43 | V29APOLICY | Character | 20 | 9a | Other insured's policy number |
| 44 | V29BRESERV | Character | 10 | 9b | Reserved |
| 45 | V29CRESERV | Character | 10 | 9c | Reserved |
| 46 | V29DINSPLA | Character | 30 | 9d | Insurance plan name |

### Condition Related To - Box 10 (V2*) - 5 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 47 | V210AEMPLO | Character | 1 | 10a | Employment related |
| 48 | V210BAUTO | Character | 1 | 10b | Auto accident |
| 49 | V210COTHER | Character | 1 | 10c | Other accident |
| 50 | V210STATE | Character | 2 | 10b | Auto accident state |
| 51 | V210DCLAIM | Character | 20 | 10d | Claim codes |

### Insured Policy - Box 11 (V2*) - 7 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 52 | V211INSURE | Character | 20 | 11 | Insured's policy group number |
| 53 | V211ABIRTH | Character | 8 | 11a | Insured's date of birth |
| 54 | V211ASEXO | Character | 1 | 11a | Insured's sex |
| 55 | V211BQUAL | Character | 2 | 11b | Other claim ID qualifier |
| 56 | V211BOTHER | Character | 30 | 11b | Other claim ID |
| 57 | V211CINSUR | Character | 30 | 11c | Insurance plan name |
| 58 | V211DISTHE | Character | 1 | 11d | Is there another health benefit plan |

### Signatures - Box 12-13 (V2*) - 3 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 59 | V212SIGNED | Character | 1 | 12 | Patient signature on file |
| 60 | V212DATE | Character | 6 | 12 | Date signed |
| 61 | V213FIRMA | Character | 1 | 13 | Insured signature on file |

### Dates - Box 14-16 (V3*) - 6 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 62 | V314DATE | Character | 6 | 14 | Date of current illness/injury |
| 63 | V314QUAL | Character | 3 | 14 | Qualifier |
| 64 | V315QUAL | Character | 3 | 15 | Other date qualifier |
| 65 | V315DATE | Character | 6 | 15 | Other date |
| 66 | V316DATEFR | Character | 6 | 16 | Dates patient unable to work - from |
| 67 | V316DATETO | Character | 6 | 16 | Dates patient unable to work - to |

### Referring Provider - Box 17 (V3*) - 5 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 68 | V317QUAL | Character | 2 | 17 | Qualifier |
| 69 | V317NAME | Character | 37 | 17 | Referring provider name |
| 70 | V317AQUAL | Character | 2 | 17a | ID qualifier |
| 71 | V317AREFFE | Character | 10 | 17a | Referring provider ID |
| 72 | V317BNPI | Character | 10 | 17b | Referring provider NPI |

### Hospitalization - Box 18-19 (V3*) - 3 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 73 | V318DATEFR | Character | 6 | 18 | Hospitalization dates - from |
| 74 | V318DATETO | Character | 6 | 18 | Hospitalization dates - to |
| 75 | V319ADD | Character | 80 | 19 | Additional claim information |

### Outside Lab - Box 20 (V3*) - 2 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 76 | V320OUTSID | Character | 1 | 20 | Outside lab |
| 77 | V320CHARGE | Character | 9 | 20 | Charges |

### Diagnosis - Box 21 (V3*) - 13 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 78 | V321ICDIND | Character | 1 | 21 | ICD indicator (9=ICD-9, 0=ICD-10) |
| 79 | V321DIAG1 | Character | 7 | 21A | Diagnosis code A |
| 80 | V321DIAG2 | Character | 7 | 21B | Diagnosis code B |
| 81 | V321DIAG3 | Character | 7 | 21C | Diagnosis code C |
| 82 | V321DIAG4 | Character | 7 | 21D | Diagnosis code D |
| 83 | V321DIAG5 | Character | 7 | 21E | Diagnosis code E |
| 84 | V321DIAG6 | Character | 7 | 21F | Diagnosis code F |
| 85 | V321DIAG7 | Character | 7 | 21G | Diagnosis code G |
| 86 | V321DIAG8 | Character | 7 | 21H | Diagnosis code H |
| 87 | V321DIAG9 | Character | 7 | 21I | Diagnosis code I |
| 88 | V321DIAG10 | Character | 7 | 21J | Diagnosis code J |
| 89 | V321DIAG11 | Character | 7 | 21K | Diagnosis code K |
| 90 | V321DIAG12 | Character | 7 | 21L | Diagnosis code L |

### Resubmission and Prior Authorization - Box 22-23 (V3*, V4*) - 3 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 91 | V322RESUB | Character | 11 | 22 | Resubmission code |
| 92 | V422ORIGIN | Character | 15 | 22 | Original ref number |
| 93 | V423PRIOR | Character | 30 | 23 | Prior authorization number |

### Tax and Patient Account - Box 25-26 (V4*) - 4 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 94 | V425FEDTAX | Character | 10 | 25 | Federal tax ID number |
| 95 | V425SSN | Character | 1 | 25 | SSN indicator |
| 96 | V425EIN | Character | 1 | 25 | EIN indicator |
| 97 | V426PATIEN | Character | 15 | 26 | Patient's account number |

### Accept Assignment and Amounts - Box 27-30 (V4*) - 4 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 98 | V427ACCEPT | Character | 1 | 27 | Accept assignment |
| 99 | V428TOTAL | Character | 9 | 28 | Total charge |
| 100 | V429AMOUNT | Character | 9 | 29 | Amount paid |
| 101 | V430NUCC | Character | 9 | 30 | Reserved for NUCC use |

### Signature of Physician - Box 31 (V4*) - 1 columna

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 102 | V431DATE | Character | 6 | 31 | Date |

### Service Facility - Box 32 (V4*) - 5 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 103 | V432FACILI | Character | 30 | 32 | Service facility name/address |
| 104 | V432ZIPCOD | Character | 5 | 32 | ZIP code |
| 105 | V432PLUS4 | Character | 4 | 32 | ZIP+4 |
| 106 | V432ANPI | Character | 10 | 32a | Service facility NPI |
| 107 | V432BOTHER | Character | 10 | 32b | Other ID |

### Billing Provider - Box 33 (V4*) - 12 columnas

| # | Field Name | Type | Length | Box | Descripcion |
|---|------------|------|--------|-----|-------------|
| 108 | V433ANPI | Character | 10 | 33a | Billing provider NPI |
| 109 | V433BQUAL | Character | 2 | 33b | Other ID qualifier |
| 110 | V433BTAXON | Character | 10 | 33b | Taxonomy code |
| 111 | V433NAME | Character | 15 | 33 | Provider first name |
| 112 | V433INITIA | Character | 1 | 33 | Provider middle initial |
| 113 | V433LASTNA | Character | 25 | 33 | Provider last name |
| 114 | V433ADDRE1 | Character | 25 | 33 | Address line 1 |
| 115 | V433ADDRE2 | Character | 25 | 33 | Address line 2 |
| 116 | V433CITY | Character | 20 | 33 | City |
| 117 | V433STATE | Character | 3 | 33 | State |
| 118 | V433ZIPCOD | Character | 5 | 33 | ZIP code |
| 119 | V433PLUS4 | Character | 4 | 33 | ZIP+4 |

---

## Service Lines - Box 24 (Fields 120-735)

28 lineas de servicio, cada una con 22 columnas = 616 columnas totales.

### Prefijos por Linea

| Linea | Prefijo | Linea | Prefijo | Linea | Prefijo | Linea | Prefijo |
|-------|---------|-------|---------|-------|---------|-------|---------|
| 1 | V5 | 8 | VC | 15 | VJ | 22 | VQ |
| 2 | V6 | 9 | VD | 16 | VK | 23 | VR |
| 3 | V7 | 10 | VE | 17 | VL | 24 | VS |
| 4 | V8 | 11 | VF | 18 | VM | 25 | VT |
| 5 | V9 | 12 | VG | 19 | VN | 26 | VU |
| 6 | VA | 13 | VH | 20 | VO | 27 | VV |
| 7 | VB | 14 | VI | 21 | VP | 28 | VW |

### Sufijos por Linea (22 columnas)

| Sufijo | Type | Length | Box | Descripcion |
|--------|------|--------|-----|-------------|
| 24ADATEF | Character | 6 | 24A | Date of service - from |
| 24ADATET | Character | 6 | 24A | Date of service - to |
| 24BPLACE | Character | 2 | 24B | Place of service |
| 24CEMG | Character | 1 | 24C | EMG (emergency) |
| 24ABBMBB | Character | 1 | - | ABB/MBB indicator |
| 24DCPT | Character | 5 | 24D | CPT/HCPCS code |
| 24DMOD1 | Character | 2 | 24D | Modifier 1 |
| 24DMOD2 | Character | 2 | 24D | Modifier 2 |
| 24DMOD3 | Character | 2 | 24D | Modifier 3 |
| 24DMOD4 | Character | 2 | 24D | Modifier 4 |
| 24EDIAGN | Character | 4 | 24E | Diagnosis pointer |
| 24FCHARG | Character | 9 | 24F | Charges |
| 24GDAYS | Character | 5 | 24G | Days or units |
| 24HEPSOT | Character | 2 | 24H | EPSDT/Family plan |
| 24IQUAL | Character | 2 | 24I | ID qualifier |
| 24JTAXON | Character | 10 | 24J | Rendering provider taxonomy |
| 24JNPI | Character | 10 | 24J | Rendering provider NPI |
| 24NDCQUA | Character | 2 | - | NDC qualifier |
| 24NDC | Character | 11 | - | NDC code |
| 24UNITQU | Character | 2 | - | Unit qualifier |
| 24UNIT | Character | 7 | - | Unit |
| CHANGE | Character | 1 | - | Change flag |

### Ejemplo: Service Line 1 (V5*)

| # | Field Name | Construccion |
|---|------------|--------------|
| 120 | V524ADATEF | V5 + 24ADATEF |
| 121 | V524ADATET | V5 + 24ADATET |
| 122 | V524BPLACE | V5 + 24BPLACE |
| ... | ... | ... |
| 141 | V5CHANGE | V5 + CHANGE |

### Ejemplo: Service Line 28 (VW*)

| # | Field Name | Construccion |
|---|------------|--------------|
| 714 | VW24ADATEF | VW + 24ADATEF |
| 715 | VW24ADATET | VW + 24ADATET |
| ... | ... | ... |
| 735 | VWCHANGE | VW + CHANGE |

---

## Implementacion en Codigo

### VdeConstants.cs (Domain/Constants/)

```csharp
// Non-service-line columns (119)
public static string[] GetNonServiceLineColumns() => [...];

// All columns including service lines (735)
public static List<string> GetAllExpectedColumns()
{
    List<string> columns = [.. GetNonServiceLineColumns()];

    for (int line = 1; line <= 28; line++)
    {
        foreach (string suffix in VdeServiceLineConstants.Suffix.GetAllSuffixes())
        {
            columns.Add(VdeServiceLineConstants.GetColumnName(line, suffix));
        }
    }

    return columns; // 119 + (28 * 22) = 735
}
```

### VdeServiceLineConstants.cs (Domain/Constants/)

```csharp
// Get prefix for line number
public static string GetPrefix(int lineNumber) => lineNumber switch
{
    1 => "V5", 2 => "V6", ..., 28 => "VW"
};

// Get full column name
public static string GetColumnName(int lineNumber, string suffix)
{
    return GetPrefix(lineNumber) + suffix;
}
```

---

## Validacion

El servicio `DbfValidationService` valida que un archivo DBF contenga las 735 columnas esperadas antes de procesar claims.

Ver: [VALIDATION_RULES.md](./VALIDATION_RULES.md#4-validación-de-estructura-dbf)
