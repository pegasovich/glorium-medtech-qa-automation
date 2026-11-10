using Serilog;

namespace Glorium.Automation.Core.Logging;

public static class TestLogger
{
    private static readonly ILogger _logger;

    static TestLogger()
    {
        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    public static void Info(string message) => _logger.Information(message);
    public static void Warn(string message) => _logger.Warning(message);
    public static void Error(string message, Exception? ex = null) => _logger.Error(ex, message);
    public static void Debug(string message) => _logger.Debug(message);
}
