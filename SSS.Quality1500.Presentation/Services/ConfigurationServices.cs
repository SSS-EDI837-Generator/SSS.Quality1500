namespace SSS.Quality1500.Presentation.Services;

using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Presentation.Interfaces;
using SSS.Quality1500.Common.Services;
using SSS.Quality1500.Domain.Interfaces;


/// <summary>
/// Implementación del agregador de servicios de configuración
/// Implementa el patrón Aggregate Services para agrupar servicios relacionados
/// Principio SRP: Se encarga únicamente de proveer servicios de configuración
/// Principio DIP: Implementa abstracción IConfigurationServices
/// </summary>
public class ConfigurationServices(
    IViewService viewService,
    IVdeRecordService vdeRecordService,
    IUiConfigurationService configurationService) : IConfigurationServices
{
    /// <summary>
    /// Servicio para manejo de vistas de la aplicación
    /// </summary>
    public IViewService ViewService { get; } = viewService ?? throw new ArgumentNullException(nameof(viewService));

    /// <summary>
    /// Servicio para lectura de archivos DBF y mapeo a VDE records
    /// </summary>
    public IVdeRecordService VdeRecordService { get; } = vdeRecordService ?? throw new ArgumentNullException(nameof(vdeRecordService));

    /// <summary>
    /// Servicio de configuración de UI
    /// </summary>
    public IUiConfigurationService ConfigurationService { get; } = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

    /// <summary>
    /// Servicio lazy para procesamiento de listas VDE
    /// Se inyectará a través de property injection en el DI container
    /// </summary>
    public LazyService<IVdeListBatch> LazyVdeListBatch { get; init; } = default!;

    /// <summary>
    /// Servicio lazy para generación de reportes de batch
    /// Se inyectará a través de property injection en el DI container
    /// </summary>
    public LazyService<IBatchReport> LazyBatchReport { get; init; } = default!;
}
