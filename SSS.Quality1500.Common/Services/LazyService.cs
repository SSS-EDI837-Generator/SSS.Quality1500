namespace SSS.Quality1500.Common.Services;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Servicio transversal que proporciona inicializacion lazy de dependencias
/// </summary>
public class LazyService<T>(IServiceProvider provider) : Lazy<T>(provider.GetRequiredService<T>)
    where T : class;
