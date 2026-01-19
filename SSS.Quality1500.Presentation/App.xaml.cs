namespace SSS.Quality1500.Presentation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SSS.Quality1500.Business.Services;
using SSS.Quality1500.Common;
using SSS.Quality1500.Presentation.Configuration;
using SSS.Quality1500.Presentation.Views;
using System.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly ServiceProvider? _serviceProvider; // ✅ Inicializado una vez en constructor

    public App()
    {
        var loggerInitializer = new LoggerInitializer();
        var environmentProvider = new EnvironmentProvider();
        var serviceConfigurator = new ServiceConfigurator();
        try
        {
            // Inicializar logger
            Result<bool, string> loggerResult = loggerInitializer.InitializeLogger();
            if (loggerResult.IsFailure)
            {
                ShowFatalError($"Error al inicializar el sistema de logs: {loggerResult.GetErrorOrDefault()}");
                return;
            }

            // Configurar servicios de la aplicación
            string environment = environmentProvider.GetEnvironment();
            _serviceProvider = serviceConfigurator.ConfigureServices(environment, out IConfiguration _);

            Log.Information("Inicialización de la aplicación finalizada correctamente.");
        }
        catch (Exception ex)
        {
            ShowFatalError($"Error fatal durante la inicialización: {ex.Message}");
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            Log.Information("Iniciando la aplicación...");

            // ✅ Verificación robusta de inicialización
            if (_serviceProvider == null)
            {
                var errorMsg = "El proveedor de servicios no está inicializado. La aplicación no puede continuar.";
                Log.Fatal(errorMsg);
                ShowFatalError(errorMsg);
                return;
            }

            // Inicializar la aplicación de forma no bloqueante
            var initTask = InitializeApplicationAsync();

            Log.Information("Aplicación iniciada correctamente.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal durante el inicio de la aplicación.");
            ShowFatalError($"Error fatal al iniciar la aplicación: {ex.Message}");
        }
    }
    private async Task InitializeApplicationAsync()
    {
        try
        {
            // ✅ Verificación defensiva adicional
            if (_serviceProvider == null)
            {
                Log.Fatal("ServiceProvider es null en InitializeApplication - esto no debería suceder");
                ShowFatalError("El proveedor de servicios no está inicializado.");
                return;
            }

            Log.Information("Obteniendo MainWindow del ServiceProvider...");

            // Inicializar ventana principal primero (UI no bloqueante)
            try
            {
                MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                Log.Information("MainWindow obtenida exitosamente");

                MainWindow.Show();
                Log.Information("Ventana principal mostrada");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error obteniendo o mostrando MainWindow");
                throw;
            }

            // Luego inicializar servicios de fondo de forma asíncrona
            _ = Task.Run(async () => {
                try
                {
                    await Task.Delay(1000); // Esperar un poco antes de inicializar servicios
                    Log.Information("Iniciando inicialización de servicios de fondo...");

                    // Crear scope para servicio Scoped
                    //using var scope = _serviceProvider.CreateScope();
                    //var initializer = scope.ServiceProvider.GetRequiredService<ApplicationInitializer>();
                    //Result<bool, string> result = await initializer.InitializeAsync();
                    //if (result.IsFailure)
                    //{
                    //    string errorMessage = result.GetErrorOrDefault() ?? "Error desconocido en la inicialización de la aplicación";
                    //    Log.Warning("Error en la inicialización de base de datos: {Error}. La aplicación continuará sin funcionalidad de DB.", errorMessage);
                    //    Log.Information("Aplicación funcionando sin base de datos - funcionalidades limitadas");
                    //}
                    //else
                    //{
                    //    Log.Information("Servicios de fondo inicializados correctamente");
                    //}
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error inesperado en la inicialización de servicios de fondo. La aplicación continuará sin DB.");
                }
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error inesperado al inicializar la aplicación");
            ShowFatalError($"Error inesperado al inicializar la aplicación: {ex.Message}");
        }
    }
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        try
        {
            Log.Information("Cerrando la aplicación...");

            _serviceProvider?.Dispose();
            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error durante el cierre de la aplicación.");
            MessageBox.Show($"Error durante el cierre de la aplicación: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
    private void ShowFatalError(string message)
    {
        MessageBox.Show(message, "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
        Current.Shutdown();
    }
}
