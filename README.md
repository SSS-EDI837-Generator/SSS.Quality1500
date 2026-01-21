# SSS.Quality1500

## Arquitectura
Este proyecto sigue **Clean Architecture** con 5 capas:

```
Common <- Domain <- Data <- Business <- Presentation
```

| Capa | Proposito |
|------|----------|
| **Domain** | Entidades, reglas de negocio, contratos (interfaces), `Result<T,E>` |
| **Business** | Casos de uso, orquestacion, LoggerInitializer |
| **Data** | Implementacion de acceso a datos (DBF, SQL, etc.) |
| **Common** | Utilidades transversales (EnvironmentProvider, Version, LazyService) |
| **Presentation** | UI con WPF + MVVM estricto |

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
