namespace SSS.Quality1500.Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


public class ApplicationInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationInitializer> _logger;

    public ApplicationInitializer(
        IConfiguration configuration,
        ILogger<ApplicationInitializer> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

}
