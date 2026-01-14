namespace SSS.Quality1500.Common.Interfaces;

public interface ILoggerInitializer
{
    Result<bool, string> InitializeLogger();
}
