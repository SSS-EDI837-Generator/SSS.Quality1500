namespace SSS.Quality1500.Data.Services;

using System.Reflection;

public class DataVersionService : Common.Version
{
    public override string GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return $"Data Layer Version: {version?.Major}.{version?.Minor}.{version?.Build}";
    }
}
