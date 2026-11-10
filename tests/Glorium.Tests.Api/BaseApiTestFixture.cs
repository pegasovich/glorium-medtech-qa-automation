using Glorium.Automation.Api.Clients;
using Glorium.Automation.Core.Configuration;
using Glorium.MedTech.MockServer;
using Microsoft.Playwright;
using NUnit.Framework;

namespace Glorium.Tests.Api;

[SetUpFixture]
public class GlobalApiSetup
{
    private static readonly string TestServerUrl = "http://127.0.0.1:5055";

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

public abstract class BaseApiTestFixture
{
    protected IPlaywright PlaywrightInstance = null!;
    protected IAPIRequestContext ApiRequestContext = null!;
    protected MedTechApiClient ApiClient = null!;

    [SetUp]
    public async Task Setup()
    {
        PlaywrightInstance = await Playwright.CreateAsync();
        ApiRequestContext = await PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = TestConfiguration.Current.BaseUrl,
            IgnoreHTTPSErrors = true
        });

        ApiClient = new MedTechApiClient(ApiRequestContext, TestConfiguration.Current.ApiBaseUrl);
    }

    [TearDown]
    public async Task Teardown()
    {
        await ApiRequestContext.DisposeAsync();
        PlaywrightInstance.Dispose();
    }
}
