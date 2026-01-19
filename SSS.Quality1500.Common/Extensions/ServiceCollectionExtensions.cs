namespace SSS.Quality1500.Common.Extensions;

using Microsoft.Extensions.DependencyInjection;
using SSS.Quality1500.Common.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(LazyService<>), typeof(LazyService<>));
        return services;
    }
}
