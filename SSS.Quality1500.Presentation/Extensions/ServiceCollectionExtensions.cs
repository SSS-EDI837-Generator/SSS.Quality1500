namespace SSS.Quality1500.Presentation.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SSS.Quality1500.Business.Extensions;
using SSS.Quality1500.Business.Services;
using SSS.Quality1500.Common.Services;
using SSS.Quality1500.Common.Extensions;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Presentation.Services;
using SSS.Quality1500.Presentation.ViewModels;
using SSS.Quality1500.Presentation.Views;
using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Domain.Interfaces;

/// <summary>
/// Extensiones para configurar servicios de la capa Presentation
/// Implementa principio Single Responsibility dividiendo configuración en módulos
/// </summary>
public static class ServiceCollectionExtensions
{

    #region Logging Extensions

    /// <summary>
    /// Configura servicios de logging
    /// Principio SRP: Solo maneja configuración de logging
    /// </summary>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        services.AddLogging(loggingBuilder => {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(dispose: true);
        });

        return services;
    }

    #endregion

    #region Core Services Extensions

    /// <summary>
    /// Configura servicios centrales de la aplicación
    /// Principio SRP: Solo maneja servicios fundamentales
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Servicios fundamentales
        services.AddSingleton(configuration);
        services.AddSingleton<ApplicationInitializer>();

        return services;
    }

    #endregion

    #region Infrastructure Services Extensions

    /// <summary>
    /// Configura servicios de infraestructura transversales
    /// Principio SRP: Solo maneja servicios de infraestructura
    /// Principio OCP: Extensible para nuevos servicios de infraestructura
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Servicios transversales
        services.AddSingleton<IViewService, ViewService>();

        // Servicio de manejo de eventos de progreso
        services.AddSingleton<ProgressEventHandlerService>(provider =>
            new ProgressEventHandlerService(
                provider.GetRequiredService<IEventAggregator>(),
                provider.GetRequiredService<ILogger<ProgressEventHandlerService>>(),
                provider.GetRequiredService<ILoggerFactory>()));

        // Configuración de UI - Usa DI exclusivamente
        services.AddSingleton<ConfigurationService>();
        services.AddSingleton<IUiConfigurationService>(provider => provider.GetRequiredService<ConfigurationService>());

        // Servicio agregado para configuración
        services.AddTransient<IConfigurationServices>(provider => {
            var configServices = new ConfigurationServices(
                provider.GetRequiredService<IViewService>(),
                provider.GetRequiredService<IVdeRecordService>(),
                provider.GetRequiredService<IUiConfigurationService>())
            {
                LazyVdeListBatch = provider.GetRequiredService<LazyService<IVdeListBatch>>(),
                LazyBatchReport = provider.GetRequiredService<LazyService<IBatchReport>>()
            };
            return configServices;
        });

        return services;
    }

    /// <summary>
    /// Configura servicios de captura de logs después del logging básico
    /// </summary>
    public static IServiceCollection AddLogCaptureServices(this IServiceCollection services)
    {
        // Servicios de log - después de configurar logging básico
        services.AddSingleton<ILogCaptureService, LogCaptureService>();
        services.AddSingleton<CaptureLoggerProvider>(provider =>
            new CaptureLoggerProvider(provider.GetRequiredService<ILogCaptureService>(), LogLevel.Debug));

        // Agregar el provider al sistema de logging
        services.AddLogging(loggingBuilder => {
            loggingBuilder.Services.AddSingleton<ILoggerProvider>(provider =>
                provider.GetRequiredService<CaptureLoggerProvider>());
        });

        return services;
    }

    #endregion

    #region UI Services Extensions

    /// <summary>
    /// Configura servicios de interfaz de usuario principales
    /// Principio SRP: Solo maneja Views y ViewModels principales (Singleton)
    /// </summary>
    public static IServiceCollection AddMainUIServices(this IServiceCollection services)
    {
        // Ventanas principales y ViewModels (Singleton para mantener estado)
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<BaseMainWindowsService>();

        return services;
    }

    /// <summary>
    /// Configura UserControls principales
    /// Principio SRP: Solo maneja UserControls que mantienen estado
    /// </summary>
    public static IServiceCollection AddUserControlServices(this IServiceCollection services)
    {
        // UserControls y ViewModels principales
        services.AddSingleton<AboutUserControl>();
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<ProcessingUserControl>();
        services.AddSingleton<ProcessingViewModel>();
        services.AddScoped<ConfigUserControl>();
        services.AddScoped<ConfigViewModel>();

        return services;
    }

    /// <summary>
    /// Configura ventanas secundarias/modales
    /// Principio SRP: Solo maneja ventanas transientes (nueva instancia cada vez)
    /// </summary>
    public static IServiceCollection AddModalWindowServices(this IServiceCollection services)
    {
        // Ventanas secundarias (Transient para crear nueva instancia cada vez)
        //services.AddTransient<ConfigurationWindow>();
        //services.AddTransient<ConfigurationViewModel>();

        return services;
    }

    /// <summary>
    /// Configura todos los servicios de UI de una vez
    /// Principio Composite: Combina múltiples configuraciones relacionadas
    /// </summary>
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        services.AddMainUIServices();
        services.AddUserControlServices();
        services.AddModalWindowServices();

        return services;
    }

    #endregion

    #region Complete Configuration Extensions

    /// <summary>
    /// Configura todos los servicios de la capa Presentation
    /// Principio Facade: Proporciona interfaz simplificada para configuración completa
    /// </summary>
    public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLoggingServices();
        services.AddLogCaptureServices(); // Agregar después del logging básico
        services.AddCoreServices(configuration);
        services.AddBusinessServices(configuration);
        services.AddCommandServices();
        services.AddInfrastructureServices();
        services.AddUIServices();

        return services;
    }

    #endregion
}
