using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SSS.Quality1500.Presentation.Configuration;

public interface IServiceConfigurator
{
    ServiceProvider ConfigureServices(string environment, out IConfiguration configuration);
}