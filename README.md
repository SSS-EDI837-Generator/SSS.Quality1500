# SSS.Quality1500

## Arquitectura
Este proyecto sigue **Onion Architecture** (Clean Architecture) con 5 capas:

```
                 ┌──────────┐
                 │  Domain  │  ← Nucleo (sin deps)
                 └──────────┘
                      ↑
         ┌───────────┴───────────┐
         │                       │
    ┌────────┐             ┌─────────┐
    │Business│             │  Data   │
    └────────┘             └─────────┘
         ↑                       ↑
         └───────────┬───────────┘
                     │
              ┌─────────────┐
              │ Presentation│  ← Composition Root
              └─────────────┘
```

| Capa | Proposito | Referencia Directa |
|------|----------|-------------------|
| **Domain** | Entidades, contratos, `Result<T,E>` | (ninguna) |
| **Common** | Utilidades transversales | (ninguna) |
| **Data** | Implementacion de acceso a datos | Domain, Common |
| **Business** | Casos de uso, CQRS handlers | **Domain** |
| **Presentation** | UI + Composition Root | **Business, Data** |

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
