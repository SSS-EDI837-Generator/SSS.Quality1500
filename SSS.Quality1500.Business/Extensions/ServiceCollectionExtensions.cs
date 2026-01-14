namespace SSS.Quality1500.Business.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SSS.Quality1500.Data.Extensions;

/// <summary>
/// Extensiones para configurar los servicios de la capa Business
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Primero registrar los servicios de la capa Data
        services.AddDataServices(configuration);

        // Registrar servicios de Business aqui
        return services;
    }
}
