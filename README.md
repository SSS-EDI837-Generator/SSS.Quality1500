# SSS.Quality1500

## Arquitectura
Este proyecto sigue **Onion Architecture** (Clean Architecture) con 5 capas:

```
        ┌─────────────────────────────────────┐
        │          PRESENTATION               │
        │  ┌───────────────────────────────┐  │
        │  │          BUSINESS             │  │
        │  │  ┌─────────────────────────┐  │  │
        │  │  │          DATA           │  │  │
        │  │  │  ┌───────────────────┐  │  │  │
        │  │  │  │      DOMAIN       │  │  │  │  ← Nucleo (sin deps)
        │  │  │  └───────────────────┘  │  │  │
        │  │  └─────────────────────────┘  │  │
        │  └───────────────────────────────┘  │
        └─────────────────────────────────────┘
```

**Cadena de dependencias (solo inmediatas):**
```
Presentation → Business → Data → Domain ← Common
```

| Capa | Proposito | Referencia Directa |
|------|----------|-------------------|
| **Domain** | Entidades, contratos, `Result<T,E>` | (ninguna) |
| **Common** | Utilidades transversales | (ninguna) |
| **Data** | Implementacion de acceso a datos | Domain, Common |
| **Business** | Casos de uso, orquestacion | Data |
| **Presentation** | UI con WPF + MVVM | Business |

Cada capa tiene un archivo `LAYER.md` con documentacion detallada.

## Inicio Rapido
```bash
cd SSS.Quality1500
dotnet restore
dotnet build
dotnet run --project SSS.Quality1500.Presentation
```

## Principios
- **SOLID** - Single Responsibility, Open/Closed, Liskov, Interface Segregation, Dependency Inversion
- **MVVM** - Model-View-ViewModel sin code-behind
- **Clean Code** - Codigo legible y mantenible
