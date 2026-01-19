namespace SSS.Quality1500.Business.Services.Interfaces;

using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Common;

/// <summary>
/// Interfaz refactorizada para StartQualityProcess
/// Implementa el patrón Parameter Object para reducir parámetros
/// </summary>
public interface IStartQualityProcessRefactored
{
    /// <summary>
    /// Procesa los datos VDE y genera archivos 837P
    /// </summary>
    /// <param name="request">Configuración del proceso</param>
    /// <returns>Resultado del procesamiento</returns>
    Task<Result<ProcessingResult, string>> StartProcess(StartProcessRequest request);
}