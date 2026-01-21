namespace SSS.Quality1500.Business.Services;

using Serilog;
using SSS.Quality1500.Domain.Interfaces;
using SSS.Quality1500.Domain.Models;

public class LoggerInitializer : ILoggerInitializer
{
    public Result<bool, string> InitializeLogger()
    {
        try
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
            string logDirectory = Path.GetDirectoryName(logPath) ?? throw new InvalidOperationException("Invalid log directory");

            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Logger initialized at {LogPath}", logPath);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool, string>.Fail($"Logger initialization failed: {ex.Message}");
        }
    }
}
