namespace SSS.Quality1500.Presentation.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SSS.Quality1500.Presentation.Extensions;
using SSS.Quality1500.Domain.Interfaces;

public class ServiceConfigurator : IServiceConfigurator
{
    /// <summary>
    /// Configura todos los servicios de la aplicación.
    /// Si se proporciona una instancia de LoggerInitializer, se reutiliza en DI (evita doble inicialización).
    /// </summary>
    /// <param name="environment">Ambiente de ejecución (Development/Production)</param>
    /// <param name="configuration">Configuración de la aplicación (out parameter)</param>
    /// <param name="loggerInitializer">Instancia opcional de LoggerInitializer para reutilizar en DI</param>
    /// <returns>ServiceProvider configurado</returns>
    public ServiceProvider ConfigureServices(string environment, out IConfiguration configuration, ILoggerInitializer? loggerInitializer = null)
    {
        // 1. Configurar y cargar configuración de la aplicación
        configuration = ConfigureApplicationConfiguration(environment);

        // 2. Crear contenedor de servicios y configurar con extensiones modulares
        var services = new ServiceCollection();

        // 3. Si se proporciona una instancia de LoggerInitializer, registrarla como Singleton
        //    Esto evita crear una segunda instancia cuando ya se inicializó en App.xaml.cs
        if (loggerInitializer != null)
        {
            services.AddSingleton(loggerInitializer);
        }

        // 4. Usar método de extensión que configura todos los servicios de forma modular
        services.AddPresentationServices(configuration);

        return services.BuildServiceProvider();
    }

    #region Configuration Setup

    /// <summary>
    /// Configura y carga la configuración de la aplicación
    /// Principio SRP: Solo maneja la carga de configuración
    /// </summary>
    private static IConfiguration ConfigureApplicationConfiguration(string environment)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string mainSettingsPath = Path.Combine(basePath, "appsettings.json");
        string envSettingsPath = Path.Combine(basePath, $"appsettings.{environment}.json");

        if (!File.Exists(mainSettingsPath))
            throw new FileNotFoundException($"Main configuration file not found: {mainSettingsPath}");

        // Solo agregar el archivo de ambiente si existe
        if (!File.Exists(envSettingsPath))
            Log.Warning("No se encontró el archivo de configuración para el ambiente {Environment} en: {Path}",
                environment, envSettingsPath);

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    #endregion
}
