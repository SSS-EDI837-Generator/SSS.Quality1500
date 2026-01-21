# GitHub Templates - Guía de Uso

Este directorio contiene los templates de GitHub para issues, pull requests y workflows de CI/CD.

## 📁 Estructura

```
.github/
├── ISSUE_TEMPLATE/
│   ├── feature.yml              # Template para features/sprints
│   ├── bug.yml                  # Template para bugs
│   ├── validation-rule.yml      # Template para reglas de validación
│   ├── documentation.yml        # Template para documentación
│   └── config.yml               # Configuración de templates
├── workflows/
│   ├── build.yml                # CI/CD: Build y tests
│   └── code-quality.yml         # CI/CD: Verificaciones de calidad
└── pull_request_template.md    # Template para PRs

```

## 🚀 Instalación

### Opción 1: Copiar manualmente
```bash
# Desde la raíz de tu repositorio SSS.Quality1500
cp -r /ruta/a/descarga/.github .
git add .github
git commit -m "chore: add GitHub templates and workflows"
git push origin main
```

### Opción 2: Crear directamente
```bash
# Crear estructura
mkdir -p .github/ISSUE_TEMPLATE
mkdir -p .github/workflows

# Copiar cada archivo individualmente desde la descarga
```

## 📝 Templates de Issues

### 1. Feature / Sprint Task (`feature.yml`)
**Cuándo usar:** Para implementar nuevas funcionalidades según PLAN_TRABAJO.md

**Campos principales:**
- Fase del proyecto (FASE 0-9)
- Sprint específico
- Capa de arquitectura afectada
- Prioridad (Alta/Media/Baja)
- Criterios de aceptación
- Archivos a crear/modificar

**Ejemplo de uso:**
```
Título: [FASE 2.1] - Implementar NpiFormatValidator
Fase: FASE 2: Business Layer - Validaciones
Sprint: Sprint 2.1: Validadores de NPI
Capa: Business
Prioridad: ⭐ Alta
```

### 2. Bug Report (`bug.yml`)
**Cuándo usar:** Para reportar errores o comportamiento inesperado

**Campos principales:**
- Severidad (Crítico/Alto/Medio/Bajo)
- Capa afectada
- Pasos para reproducir
- Comportamiento esperado vs actual
- Logs (sin PHI!)
- Entorno (OS, .NET version, etc.)

**⚠️ IMPORTANTE:** Siempre eliminar información sensible (PHI) de los logs antes de reportar.

### 3. Validation Rule (`validation-rule.yml`)
**Cuándo usar:** Para implementar nuevas reglas de validación de CMS-1500

**Campos principales:**
- Código de regla (ej: NPI-001, DATE-002)
- Categoría (NPI, DATE, CPT, ICD, etc.)
- Severidad (Critical/Error/Warning/Info)
- Regla de negocio en lenguaje natural
- Especificación técnica
- Casos de prueba (válidos e inválidos)

**Ejemplo de uso:**
```
Código: NPI-001
Nombre: NPI Format Validation
Categoría: NPI - National Provider Identifier
Severidad: Error
```

### 4. Documentation (`documentation.yml`)
**Cuándo usar:** Para crear o actualizar documentación

**Tipos de documentación:**
- README.md
- CLAUDE.md, WARP.md
- PLAN_TRABAJO.md
- Documentos en docs/ (HEALTHCARE_CONTEXT.md, VALIDATION_RULES.md, etc.)
- User Manual
- Developer Guide
- API Documentation

## 🔄 Template de Pull Request

El template de PR incluye:
- Descripción del cambio
- Link al issue relacionado
- Tipo de cambio (bug fix, feature, docs, etc.)
- Capas afectadas
- Testing realizado
- **Checklist de calidad** (obligatorio antes de merge)

### Checklist de Calidad en PRs

Antes de aprobar un PR, verificar:
- ✅ Build sin warnings
- ✅ Tests pasan
- ✅ Archivos < 500 líneas
- ✅ Máximo 3 parámetros en constructores
- ✅ Primary constructors usados
- ✅ Dependencias entre capas respetadas
- ✅ Sin PHI en logs
- ✅ Comentarios XML en APIs públicas

## ⚙️ GitHub Actions Workflows

### 1. Build and Test (`build.yml`)

**Se ejecuta en:**
- Push a `main` o `develop`
- Pull requests a `main` o `develop`

**Tareas:**
- Restaurar dependencias
- Build en modo Release con warnings como errores
- Ejecutar tests unitarios (excluyendo tests de integración)
- Generar reporte de cobertura
- Verificar formato de código

### 2. Code Quality Checks (`code-quality.yml`)

**Se ejecuta en:**
- Pull requests a `main` o `develop`

**Verificaciones:**
1. **File Size Check:**
   - Verifica que ningún archivo .cs exceda 500 líneas
   - Excluye archivos autogenerados (*.g.cs, *Designer.cs)

2. **Architecture Check:**
   - Verifica que Domain no dependa de otras capas
   - Verifica que Common no dependa de capas del proyecto
   - Verifica que Data no dependa de Business/Presentation
   - Verifica que Business no dependa de Presentation

3. **Naming Convention Check:**
   - Verifica que namespaces coincidan con estructura de carpetas

4. **HIPAA Compliance Check:**
   - Busca posibles violaciones de PHI en logs
   - Detecta logging de PatientName, DateOfBirth, Address, etc.

## 🎨 Personalización

### Agregar nuevos labels
Edita `.github/ISSUE_TEMPLATE/config.yml` para agregar links adicionales.

### Modificar workflows
Los workflows en `.github/workflows/` pueden personalizarse:
- Cambiar versión de .NET
- Agregar más verificaciones
- Integrar herramientas como SonarQube
- Agregar notificaciones (Slack, Discord, etc.)

### Crear templates adicionales
Crea nuevos archivos `.yml` en `.github/ISSUE_TEMPLATE/`:
```yaml
name: Tu Template
description: Descripción
title: "[PREFIX] - "
labels: ["tu-label"]
body:
  - type: textarea
    id: descripcion
    attributes:
      label: Descripción
    validations:
      required: true
```

## 📊 Uso Recomendado

### Workflow de Desarrollo

1. **Planificación:**
   - Revisar PLAN_TRABAJO.md
   - Crear issue usando template apropiado
   - Asignar a sprint actual

2. **Desarrollo:**
   - Crear branch desde issue: `git checkout -b feature/issue-123`
   - Implementar según criterios de aceptación
   - Verificar checklist de calidad localmente

3. **Pull Request:**
   - Crear PR usando template
   - Completar checklist de calidad
   - Esperar a que pasen los checks de CI/CD
   - Solicitar review

4. **Review y Merge:**
   - Reviewer verifica código y arquitectura
   - Si pasa review y CI/CD → Merge
   - Actualizar PLAN_TRABAJO.md si es necesario

### Creación de Issues desde PLAN_TRABAJO.md

Ejemplo: Implementar Sprint 2.1 - Validadores de NPI

```bash
# En GitHub, ir a Issues → New Issue
# Seleccionar template "Feature / Sprint Task"
# Completar:
Título: [FASE 2.1] - Implementar NpiFormatValidator
Fase: FASE 2: Business Layer - Validaciones
Sprint: Sprint 2.1: Validadores de NPI
Capa: Business
Prioridad: ⭐ Alta

Descripción:
Implementar validador de formato de NPI según VALIDATION_RULES.md

Criterios de Aceptación:
- [ ] NpiFormatValidator implementa IValidationRule<string>
- [ ] Valida que NPI sea exactamente 10 dígitos numéricos
- [ ] Tests con casos válidos e inválidos
- [ ] Coverage > 70%

Referencias:
- VALIDATION_RULES.md - Regla NPI-001
- HEALTHCARE_CONTEXT.md - Sección NPIs
```

## 🔗 Referencias

- **PLAN_TRABAJO.md** - Plan completo del proyecto
- **CLAUDE.md** - Guía de arquitectura
- **WARP.md** - Comandos y workflows
- [GitHub Docs - Issue Templates](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests/about-issue-and-pull-request-templates)
- [GitHub Actions Docs](https://docs.github.com/en/actions)

## ❓ FAQ

**P: ¿Puedo modificar los templates?**
R: Sí, puedes editar los archivos .yml según las necesidades del proyecto. Son plantillas base.

**P: ¿Los workflows de GitHub Actions cuestan?**
R: GitHub Actions es gratis para repositorios públicos. Para privados, tienes minutos gratuitos mensuales.

**P: ¿Qué hago si un workflow falla?**
R: Revisa los logs en la pestaña "Actions" del repositorio. Los errores indicarán qué verificación falló.

**P: ¿Puedo deshabilitar alguna verificación?**
R: Sí, puedes comentar o eliminar jobs específicos en los archivos de workflow.

## 📞 Soporte

Para problemas con los templates:
1. Revisar esta documentación
2. Consultar GitHub Docs
3. Crear issue con template "Documentation"

---

**Última actualización:** Enero 2026  
**Versión:** 1.0.0
