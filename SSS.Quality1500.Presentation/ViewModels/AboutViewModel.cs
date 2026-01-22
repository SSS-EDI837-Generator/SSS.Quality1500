namespace SSS.Quality1500.Presentation.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using SSS.Quality1500.Presentation.Models;


/// <summary>
/// ViewModel para la vista About que muestra información de la aplicación.
/// Sigue el principio Single Responsibility mostrando solo información de la aplicación.
/// </summary>
public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty] private string _applicationName = string.Empty;

    [ObservableProperty] private string _version = string.Empty;

    [ObservableProperty] private string _buildDate = string.Empty;

    [ObservableProperty] private string _copyright = string.Empty;

    [ObservableProperty] private string _company = string.Empty;

    [ObservableProperty] private string _description = string.Empty;

    private readonly ApplicationSettings _appSettings;

    public AboutViewModel(IOptions<ApplicationSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        LoadApplicationInfo();
    }

    /// <summary>
    /// Carga la información de la aplicación desde appsettings.json y metadatos del assembly.
    /// ApplicationName, Company y Description vienen de appsettings.json
    /// Version, BuildDate y Copyright vienen del assembly
    /// Aplica el principio DRY (Don't Repeat Yourself) centralizando la obtención de información.
    /// </summary>
    private void LoadApplicationInfo()
    {
        var assembly = Assembly.GetExecutingAssembly();
        AssemblyName assemblyName = assembly.GetName();

        // Cargar desde appsettings.json
        ApplicationName = !string.IsNullOrEmpty(_appSettings.ApplicationName)
                          ? _appSettings.ApplicationName
                          : GetAssemblyAttribute<AssemblyTitleAttribute>(assembly)?.Title ?? "My App";

        Company = !string.IsNullOrEmpty(_appSettings.CompanyName)
                  ? _appSettings.CompanyName
                  : GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly)?.Company ?? "Applica Company";

        Description = !string.IsNullOrEmpty(_appSettings.Description)
                      ? _appSettings.Description
                      : GetAssemblyAttribute<AssemblyDescriptionAttribute>(assembly)?.Description ?? "Dashboard application for processing";

        // Cargar desde assembly (mantener como estaba)
        Version = assemblyName.Version?.ToString()
                  ?? "1.0.0.0";

        BuildDate = GetBuildDate(assembly).ToString("yyyy-MM-dd HH:mm:ss");

        Copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly)?.Copyright
                    ?? $"© {DateTime.Now.Year} Dashboard";
    }

    /// <summary>
    /// Método helper para obtener atributos del assembly de forma genérica.
    /// Aplica el principio DRY y hace el código más mantenible.
    /// </summary>
    /// <typeparam name="T">Tipo de atributo a obtener</typeparam>
    /// <param name="assembly">Assembly del cual obtener el atributo</param>
    /// <returns>El atributo encontrado o null</returns>
    private static T? GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
    {
        return assembly.GetCustomAttribute<T>();
    }

    /// <summary>
    /// Obtiene la fecha de compilación del assembly.
    /// Método encapsulado que maneja la lógica específica de obtención de fecha.
    /// </summary>
    /// <param name="assembly">Assembly del cual obtener la fecha</param>
    /// <returns>Fecha de compilación</returns>
    private static DateTime GetBuildDate(Assembly assembly)
    {
        // Fallback a la fecha de creación del archivo si no se puede obtener de otra forma
        var location = assembly.Location;
        if (!string.IsNullOrEmpty(location) && System.IO.File.Exists(location))
        {
            return System.IO.File.GetCreationTime(location);
        }

        return DateTime.Now;
    }
}