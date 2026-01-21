using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSS.Quality1500.Domain.Interfaces;

namespace SSS.Quality1500.Presentation.Configuration;

public interface IServiceConfigurator
{
    /// <summary>
    /// Configura todos los servicios de la aplicación.
    /// </summary>
    /// <param name="environment">Ambiente de ejecución (Development/Production)</param>
    /// <param name="configuration">Configuración de la aplicación (out parameter)</param>
    /// <param name="loggerInitializer">Instancia opcional de LoggerInitializer para reutilizar en DI</param>
    /// <returns>ServiceProvider configurado</returns>
    ServiceProvider ConfigureServices(string environment, out IConfiguration configuration, ILoggerInitializer? loggerInitializer = null);
}