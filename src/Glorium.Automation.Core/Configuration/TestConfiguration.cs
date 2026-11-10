using Microsoft.Extensions.Configuration;

namespace Glorium.Automation.Core.Configuration;

public class TestConfiguration
{
    private static readonly Lazy<TestConfiguration> _instance = new(() => new TestConfiguration());
    public static TestConfiguration Current => _instance.Value;

    public IConfiguration Configuration { get; }

    public string BaseUrl { get; set; } = "http://localhost:5055";
    public string ApiBaseUrl { get; set; } = "http://localhost:5055/api/v1";
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; } = true;
    public float SlowMo { get; set; } = 0;
    public int DefaultTimeoutSeconds { get; set; } = 30;
    public bool EnableTracingOnFailure { get; set; } = true;

    private TestConfiguration()
    {
        var basePath = AppContext.BaseDirectory;

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Local"}.json", optional: true)
            .AddEnvironmentVariables(prefix: "GLORIUM_");

        Configuration = builder.Build();

        BaseUrl = Configuration["BaseUrl"] ?? Environment.GetEnvironmentVariable("BASE_URL") ?? BaseUrl;
        ApiBaseUrl = Configuration["ApiBaseUrl"] ?? Environment.GetEnvironmentVariable("API_BASE_URL") ?? $"{BaseUrl}/api/v1";
        Browser = Configuration["Browser"] ?? Environment.GetEnvironmentVariable("PLAYWRIGHT_BROWSER") ?? Browser;
        
        if (bool.TryParse(Configuration["Headless"] ?? Environment.GetEnvironmentVariable("HEADLESS"), out var headless))
        {
            Headless = headless;
        }

        if (int.TryParse(Configuration["DefaultTimeoutSeconds"] ?? Environment.GetEnvironmentVariable("TIMEOUT_SECONDS"), out var timeout))
        {
            DefaultTimeoutSeconds = timeout;
        }
    }
}
