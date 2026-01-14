namespace SSS.Quality1500.Common;

public abstract class Version
{
    public virtual string GetVersion()
    {
        System.Version? version = typeof(Version).Assembly.GetName().Version;
        return $"Common: {version?.Major}.{version?.Minor}.{version?.Build}";
    }
}

public class ConcreteVersion : Version
{
    public override string GetVersion()
    {
        return base.GetVersion();
    }
}
