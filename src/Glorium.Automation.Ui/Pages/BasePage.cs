using Microsoft.Playwright;

namespace Glorium.Automation.Ui.Pages;

public abstract class BasePage(IPage page)
{
    protected readonly IPage Page = page;

    public async Task NavigateToAsync(string url)
    {
        await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
    }

    public async Task WaitForPageLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task<byte[]> TakeScreenshotAsync()
    {
        return await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            FullPage = true
        });
    }
}
