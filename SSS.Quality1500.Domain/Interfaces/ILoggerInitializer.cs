namespace SSS.Quality1500.Domain.Interfaces;

using SSS.Quality1500.Domain.Models;

public interface ILoggerInitializer
{
    Result<bool, string> InitializeLogger();
}
