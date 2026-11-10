using Glorium.Automation.Api.Clients;
using Glorium.Automation.Core.Configuration;
using Glorium.MedTech.MockServer;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Glorium.Tests.E2E;

[SetUpFixture]
public class GlobalE2ESetup
{
    private static readonly string TestServerUrl = "http://127.0.0.1:5056";

    [OneTimeSetUp]
    public async Task BeforeAll()
    {
        await MockServerApp.StartInMemoryAsync(TestServerUrl);
    }

    [OneTimeTearDown]
    public async Task AfterAll()
    {
        await MockServerApp.StopInMemoryAsync(TestServerUrl);
    }
}

public abstract class BaseE2ETest : PageTest
{
    protected string BaseUrl => TestConfiguration.Current.BaseUrl;
    protected MedTechApiClient ApiClient = null!;
    private IAPIRequestContext _apiContext = null!;

    [SetUp]
    public async Task BeforeEachTest()
    {
        // 1. Initialize API client for fast preconditions and data setup
        _apiContext = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = BaseUrl
        });
        ApiClient = new MedTechApiClient(_apiContext, TestConfiguration.Current.ApiBaseUrl);

        // 2. Start Playwright Tracing for diagnostics & post-mortem analysis
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task AfterEachTest()
    {
        var testOutcome = TestContext.CurrentContext.Result.Outcome.Status;
        var testName = TestContext.CurrentContext.Test.Name;

        if (testOutcome == TestStatus.Failed)
        {
            Directory.CreateDirectory("playwright-traces");
            Directory.CreateDirectory("screenshots");

            var tracePath = Path.Combine("playwright-traces", $"{testName}-trace.zip");
            await Context.Tracing.StopAsync(new() { Path = tracePath });

            var screenshotPath = Path.Combine("screenshots", $"{testName}-failure.png");
            await Page.ScreenshotAsync(new() { Path = screenshotPath, FullPage = true });

            TestContext.AddTestAttachment(tracePath, "Playwright Trace (Upload to trace.playwright.dev)");
            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
        }
        else
        {
            await Context.Tracing.StopAsync();
        }

        await _apiContext.DisposeAsync();
    }
}
