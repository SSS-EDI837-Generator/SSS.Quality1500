namespace SSS.Quality1500.Business.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SSS.Quality1500.Business.Models;
using SSS.Quality1500.Business.Commands.ProcessClaims;
using SSS.Quality1500.Business.Queries.GetVdeRecords;
using SSS.Quality1500.Business.Queries.ValidateDate;
using SSS.Quality1500.Business.Queries.ValidateDbf;
using SSS.Quality1500.Business.Queries.ValidateIcd10;
using SSS.Quality1500.Business.Services;
using SSS.Quality1500.Business.Services.Interfaces;
using SSS.Quality1500.Domain.CQRS;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

/// <summary>
/// Extensiones para configurar los servicios de la capa Business.
/// NOTA: Esta capa solo depende de Domain. No registra servicios de Data.
/// Los servicios de Data se registran en Presentation (Composition Root).
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registrar configuración de validación DBF
        services.Configure<DbfValidationSettings>(
            configuration.GetSection(DbfValidationSettings.SectionName));

        // Registrar servicios de Business (legacy - pendiente migración a CQRS)
        services.AddTransient<IVdeRecordService, VdeRecordService>();

        // Registrar Query Handlers (CQRS)
        services.AddTransient<IQueryHandler<ValidateDbfQuery, Result<DbfValidationResult, string>>, ValidateDbfHandler>();
        services.AddTransient<IQueryHandler<GetVdeRecordsQuery, Result<List<VdeRecordDto>, string>>, GetVdeRecordsHandler>();
        services.AddTransient<IQueryHandler<ValidateDateQuery, Result<bool, FieldValidationError>>, ValidateDateHandler>();
        services.AddTransient<IQueryHandler<ValidateIcd10Query, Result<bool, FieldValidationError>>, ValidateIcd10Handler>();

        // Registrar Command Handlers (CQRS)
        services.AddTransient<ICommandHandler<ProcessClaimsCommand, Result<ClaimProcessingResult, string>>, ProcessClaimsHandler>();

        // Registrar LoggerInitializer como Singleton
        // Nota: Si ya existe una instancia en App.xaml.cs, se puede reutilizar
        services.AddSingleton<ILoggerInitializer, LoggerInitializer>();

        return services;
    }
}
