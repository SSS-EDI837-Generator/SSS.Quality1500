# Documentación Complementaria SSS.Quality1500

## 📋 Descripción

Esta carpeta contiene documentación **complementaria** para el proyecto SSS.Quality1500, diseñada para integrarse con la Onion Architecture existente documentada en `CLAUDE.md`.

## 🎯 Diferencia con la Documentación Existente

### Ya existe en tu repo:
- ✅ `CLAUDE.md` - Guía completa de Onion Architecture
- ✅ `LAYER.md` en cada capa - Documentación específica por capa
- ✅ `README.md` - Overview del proyecto

### Documentación NUEVA (complementaria):
- 📄 `HEALTHCARE_CONTEXT.md` - Contexto específico de CMS-1500 y healthcare
- 📄 `VALIDATION_RULES.md` - Reglas de negocio de validación
- 📄 `API_INTEGRATION.md` - Integración con API de validación de NPI
- 📄 `UI_PATTERNS.md` - Patrones de UI para zoom en imagen

## 📁 Archivos Incluidos

### 1. HEALTHCARE_CONTEXT.md
**Propósito:** Contexto de negocio de healthcare y formulario CMS-1500

**Contenido:**
- Descripción detallada del formulario CMS-1500
- Campos críticos y su significado
- Mapeo de campos a base de datos .dbf
- Entidades de Domain recomendadas
- Compliance HIPAA y seguridad

**Cuándo consultar:** 
- Al diseñar entidades de Domain
- Al implementar validaciones de negocio
- Al mapear datos de .dbf a modelos

### 2. VALIDATION_RULES.md
**Propósito:** Especificación de reglas de validación adaptadas a Onion Architecture

**Contenido:**
- Contratos de Domain (`IValidationRule`, `IValidator`)
- Implementaciones en Business layer
- Reglas específicas (NPI, fechas, CPT, ICD)
- Motor de validación con Result Pattern
- Testing de validadores

**Cuándo consultar:**
- Al implementar nuevas validaciones
- Al diseñar el ValidationEngine
- Al crear validadores específicos

### 3. API_INTEGRATION.md
**Propósito:** Integración con API de validación de NPI

**Contenido:**
- Contrato `INpiApiClient` en Domain
- Implementación `NpiApiClient` en Data
- Retry policies con Polly
- Caching de resultados
- Rate limiting
- Testing con mocks

**Cuándo consultar:**
- Al implementar cliente de API
- Al configurar retry policies
- Al optimizar performance con caching

### 4. UI_PATTERNS.md
**Propósito:** Patrones de UI para revisión visual con zoom

**Contenido:**
- ViewModel para revisión de errores
- Custom control `ZoomableImageControl`
- Servicio de coordenadas de campos
- Vista XAML completa
- Animaciones y pan/zoom

**Cuándo consultar:**
- Al implementar vista de revisión
- Al crear controles custom de WPF
- Al mapear campos a coordenadas de imagen

## 🏗️ Cómo Integrar con tu Proyecto

### Paso 1: Copiar Documentación

```bash
cd ~/ruta/a/SSS.Quality1500

# Crear carpeta docs si no existe
mkdir -p docs

# Copiar archivos de documentación complementaria
cp ~/SSS.Quality1500-Updated/*.md docs/
```

### Paso 2: Actualizar CLAUDE.md

Agrega esta sección al final de tu `CLAUDE.md`:

```markdown
## Documentación Complementaria

Para contexto específico de healthcare y validaciones, consulta los documentos en `docs/`:

- `docs/HEALTHCARE_CONTEXT.md` - Contexto de CMS-1500 y healthcare
- `docs/VALIDATION_RULES.md` - Reglas de validación de negocio
- `docs/API_INTEGRATION.md` - Integración con API de NPI
- `docs/UI_PATTERNS.md` - Patrones de UI para revisión visual
```

### Paso 3: Crear Project en Claude.ai

1. Ve a https://claude.ai/projects
2. Crea proyecto: **"SSS.Quality1500"**
3. Sube a Project Knowledge:
   - `CLAUDE.md` (tu archivo existente)
   - `Domain/LAYER.md` (tu archivo existente)
   - `docs/HEALTHCARE_CONTEXT.md` (nuevo)
   - `docs/VALIDATION_RULES.md` (nuevo)
   - `docs/API_INTEGRATION.md` (nuevo)
   - `docs/UI_PATTERNS.md` (nuevo)

4. Custom Instructions:
```
Soy el desarrollador principal de SSS.Quality1500, una aplicación WPF
con Onion Architecture para verificación de calidad de CMS-1500 forms.

Arquitectura Onion:
- Domain (centro, sin dependencias)
- Common (utilidades, sin dependencias)
- Business (solo Domain)
- Data (Domain y Common)
- Presentation (Business y Data - Composition Root)
- Result Pattern en Domain
- MVVM estricto con CommunityToolkit.Mvvm
- WPF con MaterialDesignThemes

Preferencias:
- Respuestas en español
- Código C# con file-scoped namespaces
- Seguir patrones definidos en CLAUDE.md
- Consultar HEALTHCARE_CONTEXT.md para contexto de negocio
- Consultar VALIDATION_RULES.md para reglas de validación

Siempre verificar que el código respete las reglas de dependencias
entre capas definidas en CLAUDE.md.
```

## 🎨 Estructura de Código Propuesta

Basada en la documentación, esta sería la estructura recomendada:

```
SSS.Quality1500/
├── Domain/
│   ├── Models/
│   │   ├── ClaimRecord.cs           # [NUEVO] Entidad principal
│   │   ├── PatientInfo.cs           # [NUEVO] Value object
│   │   ├── ServiceInfo.cs           # [NUEVO] Value object
│   │   ├── ProviderInfo.cs          # [NUEVO] Value object
│   │   └── ValidationError.cs       # [NUEVO] Modelo de error
│   ├── Enums/
│   │   ├── ReviewStatus.cs          # [NUEVO]
│   │   ├── ErrorSeverity.cs         # [NUEVO]
│   │   └── Gender.cs                # [NUEVO]
│   ├── Interfaces/
│   │   ├── IValidator.cs            # [NUEVO] Contrato de validador
│   │   ├── IValidationRule.cs       # [NUEVO] Contrato de regla
│   │   ├── INpiApiClient.cs         # [NUEVO] Contrato de API client
│   │   └── IImageService.cs         # [NUEVO] Servicio de imágenes
│   └── LAYER.md                     # [EXISTENTE]
│
├── Data/
│   ├── ApiClients/
│   │   ├── NpiApiClient.cs          # [NUEVO] Implementación HTTP
│   │   └── CachedNpiApiClient.cs    # [NUEVO] Con caching
│   ├── Models/
│   │   ├── NpiValidationRequest.cs  # [NUEVO]
│   │   └── NpiValidationResponse.cs # [NUEVO]
│   └── LAYER.md                     # [EXISTENTE]
│
├── Business/
│   ├── Validators/
│   │   ├── NpiFormatValidator.cs    # [NUEVO]
│   │   ├── NpiExistenceValidator.cs # [NUEVO]
│   │   ├── NpiChecksumValidator.cs  # [NUEVO]
│   │   ├── DateFormatValidator.cs   # [NUEVO]
│   │   └── DateRangeValidator.cs    # [NUEVO]
│   ├── Services/
│   │   ├── ValidationEngine.cs      # [NUEVO] Motor de validación
│   │   └── ImageService.cs          # [NUEVO] Coordenadas de campos
│   └── LAYER.md                     # [EXISTENTE]
│
├── Presentation/
│   ├── ViewModels/
│   │   └── ErrorReviewViewModel.cs  # [NUEVO]
│   ├── Views/
│   │   └── ErrorReviewView.xaml     # [NUEVO]
│   ├── Controls/
│   │   └── ZoomableImageControl.xaml # [NUEVO]
│   └── LAYER.md                     # [EXISTENTE]
│
├── docs/                            # [NUEVO]
│   ├── HEALTHCARE_CONTEXT.md
│   ├── VALIDATION_RULES.md
│   ├── API_INTEGRATION.md
│   └── UI_PATTERNS.md
│
├── CLAUDE.md                        # [EXISTENTE]
├── README.md                        # [EXISTENTE]
└── WARP.md                          # [EXISTENTE]
```

## 💡 Ejemplos de Uso

### Ejemplo 1: Implementar Validación de NPI

**Pregunta a Claude:**
```
Necesito implementar la validación NPI-002 (existencia en BD).
Siguiendo la arquitectura definida en CLAUDE.md y las reglas 
en VALIDATION_RULES.md, ¿cómo debería hacerlo?
```

**Claude consultará:**
1. `CLAUDE.md` - Para entender Clean Architecture y Result Pattern
2. `VALIDATION_RULES.md` - Para ver la especificación exacta de NPI-002
3. `API_INTEGRATION.md` - Para usar `INpiApiClient`

### Ejemplo 2: Crear Entidad de Domain

**Pregunta a Claude:**
```
Necesito crear la entidad ClaimRecord en Domain/Models.
¿Qué campos debería tener basándome en el CMS-1500?
```

**Claude consultará:**
1. `HEALTHCARE_CONTEXT.md` - Para estructura del formulario CMS-1500
2. `CLAUDE.md` - Para reglas de Domain layer
3. `Domain/LAYER.md` - Para ejemplos de entidades

### Ejemplo 3: Implementar Vista de Revisión

**Pregunta a Claude:**
```
Necesito crear la vista de revisión de errores con zoom
en imagen. ¿Cómo debería estructurar el ViewModel?
```

**Claude consultará:**
1. `UI_PATTERNS.md` - Para patrones específicos de zoom
2. `CLAUDE.md` - Para patrones MVVM estrictos
3. `Presentation/LAYER.md` - Para reglas de UI layer

## ✅ Checklist de Implementación

### Fase 1: Setup (Esta Semana)
- [ ] Copiar documentación complementaria a `docs/`
- [ ] Crear Project en Claude.ai con toda la documentación
- [ ] Implementar entidades básicas en Domain
  - [ ] ClaimRecord
  - [ ] PatientInfo
  - [ ] ServiceInfo
  - [ ] ProviderInfo
  - [ ] ValidationError

### Fase 2: Validaciones (Próxima Semana)
- [ ] Implementar contratos en Domain
  - [ ] IValidationRule<T>
  - [ ] IValidator<T>
- [ ] Implementar validadores en Business
  - [ ] NpiFormatValidator
  - [ ] NpiExistenceValidator
  - [ ] DateFormatValidator
- [ ] Implementar ValidationEngine

### Fase 3: API Integration
- [ ] Implementar INpiApiClient en Domain
- [ ] Implementar NpiApiClient en Data
- [ ] Configurar Polly retry policies
- [ ] Agregar caching con CachedNpiApiClient

### Fase 4: UI
- [ ] Crear ErrorReviewViewModel
- [ ] Crear ZoomableImageControl
- [ ] Implementar ImageService
- [ ] Crear ErrorReviewView

## 🔗 Referencias Cruzadas

| Tema | CLAUDE.md | Docs Complementarias |
|------|-----------|---------------------|
| Arquitectura General | ✅ Sección "Architecture" | - |
| Dependencias entre capas | ✅ "Dependency Matrix" | - |
| Result Pattern | ✅ "Result Pattern" | VALIDATION_RULES.md |
| Entidades de Domain | ✅ Domain/LAYER.md | HEALTHCARE_CONTEXT.md |
| Validaciones | - | VALIDATION_RULES.md |
| API Client | ✅ "Tech Stack" | API_INTEGRATION.md |
| UI/MVVM | ✅ "MVVM Implementation" | UI_PATTERNS.md |
| Healthcare Context | - | HEALTHCARE_CONTEXT.md |

## 🚀 Próximos Pasos

1. **Revisar** toda la documentación complementaria
2. **Integrar** los archivos en tu repo bajo `docs/`
3. **Crear Project** en Claude.ai con toda la documentación
4. **Empezar** con la implementación de entidades de Domain
5. **Usar Claude Code** en terminal para coding asistido

---

**Nota:** Esta documentación está diseñada para **complementar** (no reemplazar) la excelente arquitectura que ya tienes documentada en CLAUDE.md y tus LAYER.md files. El objetivo es agregar contexto específico de healthcare y patrones de implementación sin duplicar información.

¿Listo para empezar a codificar con contexto completo? 🎉
