namespace SSS.Quality1500.Presentation.Interfaces;

using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Common.Services;
using SSS.Quality1500.Domain.Interfaces;


/// <summary>
/// Interfaz que agrupa servicios relacionados con la configuración del sistema
/// Implementa el patrón Aggregate Services para reducir dependencias del constructor
/// Principio SRP: Se encarga únicamente de proveer servicios de configuración
/// Principio ISP: Interfaz específica para servicios de configuración
/// </summary>
public interface IConfigurationServices
{
    /// <summary>
    /// Servicio para manejo de vistas de la aplicación
    /// </summary>
    IViewService ViewService { get; }

    /// <summary>
    /// Servicio para lectura de archivos DBF y mapeo a VDE records
    /// </summary>
    IVdeRecordService VdeRecordService { get; }

    /// <summary>
    /// Servicio lazy para procesamiento de listas VDE
    /// </summary>
    LazyService<IVdeListBatch> LazyVdeListBatch { get; }

    /// <summary>
    /// Servicio lazy para generación de reportes de batch
    /// </summary>
    LazyService<IBatchReport> LazyBatchReport { get; }

    /// <summary>
    /// Servicio de configuración de UI
    /// </summary>
    IUiConfigurationService ConfigurationService { get; }
}
