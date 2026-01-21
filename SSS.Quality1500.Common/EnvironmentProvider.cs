namespace SSS.Quality1500.Common;

using SSS.Quality1500.Common.Interfaces;

public class EnvironmentProvider : IEnvironmentProvider
{
    public string GetEnvironment()
    {
#if DEBUG
        return "Development";
#else
        return "Production";
#endif
    }
}
