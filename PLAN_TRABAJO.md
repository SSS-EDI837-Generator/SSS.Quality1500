# Plan de Trabajo - SSS.Quality1500

## 📋 Resumen Ejecutivo

**Proyecto:** SSS.Quality1500  
**Repositorio:** https://github.com/SSS-EDI837-Generator/SSS.Quality1500  
**Objetivo:** Aplicación WPF para verificación de calidad de formularios CMS-1500  
**Arquitectura:** Clean Architecture (5 capas) + MVVM estricto  
**Estado:** Proyecto iniciado, requiere desarrollo completo  

---

## 🎯 Fases del Proyecto

### **FASE 0: Setup y Fundamentos** (1 semana)
**Objetivo:** Establecer la infraestructura base del proyecto

#### Sprint 0.1: Estructura de Proyecto
- [X] Verificar estructura de carpetas según Clean Architecture
- [X] Configurar proyectos (.csproj) con dependencias correctas
- [X] Implementar `Result<T,E>` en Domain/Models/
- [X] Configurar DI en cada capa (ServiceCollectionExtensions)
- [ ] Implementar EnvironmentProvider y Version en Common

#### Sprint 0.2: Logging y Configuración
- [ ] Implementar LoggerInitializer en Business/Services/
- [ ] Configurar Serilog con file sink y console sink
- [ ] Crear appsettings.json base (Development, Production)
- [ ] Implementar ServiceConfigurator en Presentation
- [ ] Configurar MaterialDesignThemes en App.xaml

#### Sprint 0.3: CI/CD y Control de Calidad
- [ ] Configurar GitHub Actions para build
- [ ] Implementar pre-commit hooks (file size, warnings)
- [ ] Configurar EditorConfig para estándares de código
- [ ] Crear plantillas de PR y Issues
- [ ] Documentar Git workflow en README.md

**Entregables:**
- ✅ Proyecto compila sin warnings
- ✅ Logs funcionando correctamente
- ✅ CI/CD pipeline operativo

---

### **FASE 1: Core Domain y Data Access** (2 semanas)

#### Sprint 1.1: Domain Layer - Entidades Base
**Objetivo:** Implementar entidades del dominio según HEALTHCARE_CONTEXT.md

```csharp
Domain/
├── Models/
│   ├── ClaimRecord.cs          ⭐ PRIORIDAD ALTA
│   ├── PatientInfo.cs          ⭐ PRIORIDAD ALTA
│   ├── ServiceInfo.cs          ⭐ PRIORIDAD ALTA
│   ├── ProviderInfo.cs         ⭐ PRIORIDAD ALTA
│   └── ValidationError.cs      ⭐ PRIORIDAD ALTA
├── Enums/
│   ├── ReviewStatus.cs
│   ├── ErrorSeverity.cs
│   └── Gender.cs
└── Interfaces/
    └── IDbfReader.cs            ⭐ PRIORIDAD ALTA
```

**Tareas:**
- [ ] Implementar ClaimRecord con reglas de validación intrínsecas
- [ ] Implementar Value Objects (PatientInfo, ServiceInfo, ProviderInfo)
- [ ] Crear enumeraciones de negocio
- [ ] Definir contrato IDbfReader con métodos de lectura/escritura
- [ ] Tests unitarios para entidades (Domain.Tests)

#### Sprint 1.2: Data Layer - DBF Reader
**Objetivo:** Implementar lectura de archivos .dbf según estructura VDE

```csharp
Data/
├── Services/
│   └── DbfReader.cs             ⭐ PRIORIDAD ALTA
├── Models/
│   └── VdeRecord.cs
└── Extensions/
    └── ServiceCollectionExtensions.cs
```

**Tareas:**
- [ ] Implementar DbfReader con NDbfReader
- [ ] Mapear campos DBF a ClaimRecord
- [ ] Manejar encoding y formatos de fecha
- [ ] Implementar conversión DataTable → List<ClaimRecord>
- [ ] Tests de integración con archivos .dbf de prueba

#### Sprint 1.3: Data Layer - DBF Writer
**Objetivo:** Escritura y actualización de archivos DBF

```csharp
Data/
└── Services/
    └── DbfReader.cs             ⭐ PRIORIDAD MEDIA (ampliar)
```

**Tareas:**
- [ ] Implementar UpdateRecordAsync en DbfReader
- [ ] Implementar escritura de campos modificados
- [ ] Manejar bloqueo de archivos durante escritura
- [ ] Backup automático antes de modificar
- [ ] Tests de integración de escritura DBF

**Entregables:**
- ✅ Lectura de archivos .dbf funcional
- ✅ Escritura de archivos .dbf operativa
- ✅ Tests de Data layer pasando

---

### **FASE 2: Business Layer - Validaciones** (3 semanas)

#### Sprint 2.1: Validadores de NPI (Fase 1 de VALIDATION_RULES.md)
**Objetivo:** Implementar validaciones críticas de NPI

```csharp
Business/
├── Validators/
│   ├── NpiFormatValidator.cs    ⭐ PRIORIDAD ALTA
│   ├── NpiChecksumValidator.cs  ⭐ PRIORIDAD ALTA
│   └── NpiExistenceValidator.cs ⭐ PRIORIDAD ALTA
└── Services/
    └── ValidationEngine.cs      ⭐ PRIORIDAD ALTA
```

**Tareas:**
- [ ] Implementar NPI-001: Validación de formato (10 dígitos)
- [ ] Implementar NPI-003: Checksum Luhn
- [ ] Implementar NPI-002: Mock para existencia (sin API aún)
- [ ] Crear ValidationEngine que orqueste reglas
- [ ] Tests unitarios completos para cada validador

#### Sprint 2.2: Validadores de Fechas (Fase 1 de VALIDATION_RULES.md)
**Objetivo:** Validaciones de fechas de servicio

```csharp
Business/
└── Validators/
    ├── DateFormatValidator.cs   ⭐ PRIORIDAD ALTA
    ├── DateNotFutureValidator.cs⭐ PRIORIDAD ALTA
    └── DateRangeValidator.cs    ⭐ PRIORIDAD ALTA
```

**Tareas:**
- [ ] Implementar DATE-001: Formato de fecha válido
- [ ] Implementar DATE-002: Fecha no futura
- [ ] Implementar DATE-003: Rango DateFrom ≤ DateTo
- [ ] Integrar validadores de fecha al ValidationEngine
- [ ] Tests con casos edge (leap years, DST, etc.)

#### Sprint 2.3: Servicio de Procesamiento de Claims
**Objetivo:** Orquestar validación completa de claims

```csharp
Business/
└── Services/
    ├── ClaimProcessingService.cs ⭐ PRIORIDAD ALTA
    └── BatchProcessingService.cs
```

**Tareas:**
- [ ] Implementar ClaimProcessingService
  - Leer claims desde DBF
  - Ejecutar ValidationEngine
  - Guardar correcciones en DBF
- [ ] Implementar procesamiento por lotes
- [ ] Agregar progreso y cancelación (IProgress, CancellationToken)
- [ ] Tests de integración end-to-end

**Entregables:**
- ✅ Validaciones Fase 1 completadas (NPI + Fechas)
- ✅ Motor de validación operativo
- ✅ Pipeline de procesamiento funcional

---

### **FASE 3: API Integration** (2 semanas)

#### Sprint 3.1: Cliente HTTP para API de NPI
**Objetivo:** Implementar integración según API_INTEGRATION.md

```csharp
Data/
├── ApiClients/
│   ├── NpiApiClient.cs          ⭐ PRIORIDAD ALTA
│   ├── CachedNpiApiClient.cs
│   └── RateLimitedNpiApiClient.cs
└── Models/
    ├── NpiValidationRequest.cs
    └── NpiValidationResponse.cs
```

**Tareas:**
- [ ] Implementar NpiApiClient con HttpClient
- [ ] Configurar Polly (retry policy + circuit breaker)
- [ ] Implementar CachedNpiApiClient con IMemoryCache
- [ ] Implementar rate limiting con SemaphoreSlim
- [ ] Configurar appsettings con endpoints de API

#### Sprint 3.2: Integración con Validadores
**Objetivo:** Conectar NpiExistenceValidator con API real

**Tareas:**
- [ ] Actualizar NpiExistenceValidator para usar INpiApiClient
- [ ] Implementar retry logic en validador
- [ ] Agregar logging de llamadas API
- [ ] Tests de integración con API mock (WireMock)
- [ ] Tests de integración con API real (opcional, marcados como [Integration])

**Entregables:**
- ✅ API client funcional con resiliencia
- ✅ Caching y rate limiting operativos
- ✅ Validación NPI-002 conectada a API real

---

### **FASE 4: Presentation Layer - UI Base** (2 semanas)

#### Sprint 4.1: Shell y Navegación
**Objetivo:** Estructura base de la aplicación WPF

```csharp
Presentation/
├── Views/
│   ├── ShellView.xaml           ⭐ PRIORIDAD ALTA
│   └── MainWindow.xaml
├── ViewModels/
│   ├── ShellViewModel.cs
│   └── MainViewModel.cs
└── Services/
    ├── NavigationService.cs     ⭐ PRIORIDAD MEDIA
    └── DialogService.cs
```

**Tareas:**
- [ ] Implementar ShellView con MaterialDesignThemes
- [ ] Crear NavigationService para vistas
- [ ] Implementar DialogService para confirmaciones/errores
- [ ] Configurar ViewModelLocator
- [ ] Implementar menú principal y navegación básica

#### Sprint 4.2: Vista de Carga de Archivos
**Objetivo:** UI para seleccionar y cargar archivos .dbf

```csharp
Presentation/
├── Views/
│   └── FileLoadView.xaml
└── ViewModels/
    └── FileLoadViewModel.cs     ⭐ PRIORIDAD ALTA
```

**Tareas:**
- [ ] Implementar FileLoadView con drag-drop
- [ ] FileLoadViewModel con comando para seleccionar archivo
- [ ] Mostrar preview de registros cargados
- [ ] Barra de progreso para carga de archivos grandes
- [ ] Binding bidireccional con UpdateSourceTrigger=PropertyChanged

**Entregables:**
- ✅ Aplicación WPF ejecutándose
- ✅ Navegación entre vistas funcional
- ✅ Carga de archivos .dbf desde UI

---

### **FASE 5: UI de Revisión de Errores** (3 semanas)

#### Sprint 5.1: Servicio de Imágenes
**Objetivo:** Implementar manejo de imágenes según UI_PATTERNS.md

```csharp
Business/
└── Services/
    └── ImageService.cs          ⭐ PRIORIDAD ALTA

Domain/
└── Interfaces/
    └── IImageService.cs
```

**Tareas:**
- [ ] Implementar IImageService en Domain
- [ ] Implementar ImageService con coordenadas de campos
- [ ] Mapear coordenadas de CMS-1500 (Box 17b, 24J, 33a, etc.)
- [ ] Método para cargar imágenes (async)
- [ ] Tests con imágenes de prueba

#### Sprint 5.2: Control ZoomableImageControl
**Objetivo:** Control WPF personalizado para zoom interactivo

```csharp
Presentation/
└── Controls/
    ├── ZoomableImageControl.xaml     ⭐ PRIORIDAD ALTA
    └── ZoomableImageControl.xaml.cs
```

**Tareas:**
- [ ] Implementar ZoomableImageControl según UI_PATTERNS.md
- [ ] Zoom con mouse wheel
- [ ] Pan con mouse drag
- [ ] Highlight de área seleccionada
- [ ] Animaciones suaves (Storyboards)
- [ ] Dependency Properties para binding

#### Sprint 5.3: Vista de Revisión de Errores
**Objetivo:** Vista principal para revisar errores con zoom

```csharp
Presentation/
├── Views/
│   └── ErrorReviewView.xaml     ⭐ PRIORIDAD ALTA
└── ViewModels/
    └── ErrorReviewViewModel.cs  ⭐ PRIORIDAD ALTA
```

**Tareas:**
- [ ] Implementar ErrorReviewViewModel según UI_PATTERNS.md
- [ ] Comandos: NextError, PreviousError, AcceptError, CorrectField
- [ ] Binding con ZoomableImageControl
- [ ] Panel de campos editables con zoom automático al hacer focus
- [ ] Navegación entre errores con teclado (shortcuts)

**Entregables:**
- ✅ ZoomableImageControl funcional
- ✅ Vista de revisión operativa
- ✅ Experiencia de usuario fluida para corrección de errores

---

### **FASE 6: Validaciones Avanzadas** (2 semanas)

#### Sprint 6.1: Validadores CPT/HCPCS (Fase 2 de VALIDATION_RULES.md)
**Objetivo:** Validación de códigos de procedimiento

```csharp
Business/
└── Validators/
    ├── CptFormatValidator.cs
    └── HcpcsFormatValidator.cs
```

**Tareas:**
- [ ] Implementar CPT-001: Formato de código CPT (5 dígitos)
- [ ] Validar HCPCS (1 letra + 4 dígitos)
- [ ] Validar modificadores (2 caracteres, hasta 4)
- [ ] Integrar al ValidationEngine

#### Sprint 6.2: Validadores ICD-10 (Fase 2)
**Objetivo:** Validación de códigos de diagnóstico

```csharp
Business/
└── Validators/
    ├── IcdFormatValidator.cs
    └── DiagnosisPointerValidator.cs
```

**Tareas:**
- [ ] Implementar ICD-001: Formato de ICD-10 (3-7 caracteres)
- [ ] Validar punto decimal en posición correcta
- [ ] Implementar ICD-002: Diagnosis Pointer válido (referencia a Box 21)
- [ ] Verificar que pointer apunte a código existente

#### Sprint 6.3: Validadores de Montos (Fase 2)
**Objetivo:** Validación de cantidades monetarias

```csharp
Business/
└── Validators/
    └── AmountValidator.cs
```

**Tareas:**
- [ ] Implementar AMT-001: Formato de monto (positivo, 2 decimales)
- [ ] Validar rango razonable (no negativo, no excesivo)
- [ ] Validar suma de líneas vs total

**Entregables:**
- ✅ Validaciones Fase 2 completadas
- ✅ Cobertura de validación > 80%

---

### **FASE 7: Features Avanzadas** (2 semanas)

#### Sprint 7.1: Exportación de Resultados
**Objetivo:** Exportar reportes de validación

```csharp
Business/
└── Services/
    ├── ExcelExportService.cs
    └── PdfReportService.cs
```

**Tareas:**
- [ ] Implementar exportación a Excel (EPPlus o similar)
- [ ] Generar reporte PDF con resumen de errores
- [ ] Exportar imágenes con errores marcados
- [ ] Vista de exportación en UI

#### Sprint 7.2: Batch Processing UI
**Objetivo:** Procesamiento masivo de claims

```csharp
Presentation/
├── Views/
│   └── BatchProcessingView.xaml
└── ViewModels/
    └── BatchProcessingViewModel.cs
```

**Tareas:**
- [ ] Vista para seleccionar múltiples archivos .dbf
- [ ] Barra de progreso con % completado
- [ ] Procesamiento en background (Task.Run)
- [ ] Cancelación de batch (CancellationTokenSource)
- [ ] Resumen de resultados al finalizar

#### Sprint 7.3: Configuración de Reglas
**Objetivo:** Hacer validaciones configurables

```csharp
Business/
└── Services/
    └── ValidationConfigService.cs
```

**Tareas:**
- [ ] Cargar reglas desde appsettings.json
- [ ] UI para habilitar/deshabilitar reglas
- [ ] Configurar severidad de reglas (Error, Warning, Info)
- [ ] Persistir configuración del usuario

**Entregables:**
- ✅ Exportación de reportes funcional
- ✅ Batch processing operativo
- ✅ Configuración flexible de validaciones

---

### **FASE 8: Testing y Calidad** (2 semanas)

#### Sprint 8.1: Tests de Integración
**Objetivo:** Cobertura de tests end-to-end

**Tareas:**
- [ ] Tests de integración: DBF → Validation → DBF
- [ ] Tests de integración: API NPI con WireMock
- [ ] Tests de UI con FlaUI o similar
- [ ] Cobertura de código > 70%

#### Sprint 8.2: Performance y Optimización
**Objetivo:** Optimizar rendimiento

**Tareas:**
- [ ] Profiling con dotTrace o VS Profiler
- [ ] Optimizar lectura/escritura DBF (buffering, batch updates)
- [ ] Implementar paginación en UI (VirtualizingStackPanel)
- [ ] Async/await en todas las operaciones I/O
- [ ] Memory leak detection

#### Sprint 8.3: HIPAA Compliance Review
**Objetivo:** Verificar cumplimiento de seguridad

**Tareas:**
- [ ] Audit de logging (sin PHI en logs)
- [ ] Protección de archivos DBF sensibles
- [ ] Implementar audit trail de cambios
- [ ] Session timeout automático
- [ ] Revisión de código por seguridad

**Entregables:**
- ✅ Tests de integración completos
- ✅ Performance optimizado
- ✅ Compliance con HIPAA verificado

---

### **FASE 9: Deployment y Documentación** (1 semana)

#### Sprint 9.1: Packaging y Deployment
**Objetivo:** Preparar para distribución

**Tareas:**
- [ ] Configurar ClickOnce o MSIX para deployment
- [ ] Crear instalador con Wix o Inno Setup
- [ ] Configurar auto-update mechanism
- [ ] Build Release con optimizaciones
- [ ] Crear paquete de distribución

#### Sprint 9.2: Documentación
**Objetivo:** Documentar proyecto completo

**Tareas:**
- [ ] Actualizar README.md con instrucciones completas
- [ ] Documentar arquitectura con diagramas (PlantUML)
- [ ] User manual en docs/
- [ ] Developer guide en docs/
- [ ] API documentation con XML comments

#### Sprint 9.3: Release Preparation
**Objetivo:** Preparar versión 1.0.0

**Tareas:**
- [ ] Code review final
- [ ] Testing de aceptación con usuario final
- [ ] Release notes v1.0.0
- [ ] Tag y GitHub Release
- [ ] Deployment a producción

**Entregables:**
- ✅ Aplicación lista para producción
- ✅ Documentación completa
- ✅ Release v1.0.0 publicado

---

## 📊 Roadmap Visual

```
Q1 2026
├─ Semana 1-2:  FASE 0 + FASE 1 Sprint 1.1
├─ Semana 3-4:  FASE 1 Sprint 1.2-1.3
└─ Semana 5:    FASE 2 Sprint 2.1

Q2 2026
├─ Semana 6-7:  FASE 2 Sprint 2.2-2.3
├─ Semana 8-9:  FASE 3
├─ Semana 10-11: FASE 4
└─ Semana 12-13: FASE 5 Sprint 5.1-5.2

Q3 2026
├─ Semana 14:   FASE 5 Sprint 5.3
├─ Semana 15-16: FASE 6
├─ Semana 17-18: FASE 7
└─ Semana 19-20: FASE 8

Q4 2026
├─ Semana 21:   FASE 9
└─ Semana 22+:  Mantenimiento y nuevas features
```

---

## 🎯 Prioridades y Quick Wins

### Prioridad Alta (Semanas 1-8)
1. ✅ Domain entities (ClaimRecord, PatientInfo, etc.)
2. ✅ DbfReader funcional
3. ✅ Validadores NPI (Fase 1)
4. ✅ Validadores de fechas (Fase 1)
5. ✅ API Integration básica
6. ✅ UI de carga de archivos

### Prioridad Media (Semanas 9-16)
7. ⏳ ZoomableImageControl
8. ⏳ ErrorReviewView completa
9. ⏳ Validadores CPT/ICD (Fase 2)
10. ⏳ Batch processing

### Prioridad Baja (Semanas 17+)
11. ⏳ Exportación avanzada
12. ⏳ Configuración de reglas
13. ⏳ Auto-update mechanism

---

## ✅ Checklist de Calidad (Cada Sprint)

Antes de completar cada sprint, verificar:

- [ ] Código compila sin warnings (`dotnet build`)
- [ ] Tests unitarios pasan (coverage > 70%)
- [ ] Código sigue convenciones (file size < 500 líneas)
- [ ] Máximo 3 parámetros en constructores
- [ ] Primary constructors usados donde sea posible
- [ ] Namespaces coinciden con estructura de carpetas
- [ ] Dependencias entre capas respetadas
- [ ] Logging sin PHI (HIPAA compliance)
- [ ] Comentarios XML en APIs públicas
- [ ] PR review completado (si aplica)

---

## 📈 Métricas de Éxito

### Métricas Técnicas
- **Cobertura de tests:** > 70%
- **Code smells:** 0 (SonarQube)
- **Bugs críticos:** 0
- **Tiempo de build:** < 2 minutos
- **Tamaño de archivos:** < 500 líneas promedio

### Métricas de Negocio
- **Precisión de validación:** > 95%
- **Tiempo de procesamiento:** < 1 segundo por claim
- **False positives:** < 5%
- **User satisfaction:** > 4.5/5

---

## 🚀 Comandos de Desarrollo

```bash
# Setup inicial
git clone https://github.com/SSS-EDI837-Generator/SSS.Quality1500
cd SSS.Quality1500
dotnet restore

# Build y Run
dotnet build
dotnet run --project SSS.Quality1500.Presentation

# Testing
dotnet test --collect:"XPlat Code Coverage"
dotnet test --filter "Category!=Integration"

# Release
dotnet build -c Release
dotnet publish -c Release -r win-x64 --self-contained

# Code analysis
dotnet format --verify-no-changes
dotnet build /p:TreatWarningsAsErrors=true
```

---

## 📞 Contacto y Recursos

- **Repository:** https://github.com/SSS-EDI837-Generator/SSS.Quality1500
- **Documentation:** `docs/` folder
- **Issues:** GitHub Issues
- **Pull Requests:** Seguir Git workflow en WARP.md

---

## 📝 Notas de Seguimiento

### Estado Actual (Actualizar semanalmente)
- **Fase Actual:** _____
- **Sprint Actual:** _____
- **Progreso General:** ____%
- **Bloqueadores:** _____
- **Próximos Pasos:** _____

### Cambios al Plan
| Fecha | Cambio | Razón |
|-------|--------|-------|
| | | |

---

**Nota Final:** Este plan es iterativo. Al completar cada fase, revisar y ajustar prioridades basándose en feedback del usuario y hallazgos técnicos.
