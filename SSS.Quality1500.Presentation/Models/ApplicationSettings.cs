namespace SSS.Quality1500.Presentation.Models;

/// <summary>
/// Modelo de configuración para información de la aplicación
/// Mapea la sección "ApplicationSettings" del appsettings.json
/// Se usa para configurar información de presentación como título de ventana, pantalla About, etc.
/// </summary>
public class ApplicationSettings
{
    /// <summary>
    /// Nombre de la aplicación mostrado en la UI
    /// </summary>
    public string ApplicationName { get; set; } = string.Empty;

    /// <summary>
    /// Versión de la aplicación
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la compañía desarrolladora
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Email de soporte técnico
    /// </summary>
    public string SupportEmail { get; set; } = string.Empty;

    /// <summary>
    /// Descripción de la aplicación
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
