namespace SSS.Quality1500.Data.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Data.Services;
using SSS.Quality1500.Data.Repositories;

/// <summary>
/// Extensiones para configurar los servicios de la capa Data.
/// Registra implementaciones de contratos definidos en Domain.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // IDbfReader es un contrato de Domain, DbfReader es la implementacion en Data
        services.AddTransient<IDbfReader, DbfReader>();
        services.AddTransient<DataVersionService>();

        // Repositorio de configuracion de columnas (singleton para cache)
        services.AddSingleton<IColumnConfigurationRepository, JsonColumnConfigurationRepository>();

        return services;
    }
}
