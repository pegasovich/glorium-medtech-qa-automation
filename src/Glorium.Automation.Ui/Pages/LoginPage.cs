using Microsoft.Playwright;

namespace Glorium.Automation.Ui.Pages;

public class LoginPage(IPage page) : BasePage(page)
{
    public ILocator HeaderLocator => Page.GetByTestId("portal-header");
    public ILocator UsernameInput => Page.GetByTestId("input-username");
    public ILocator PasswordInput => Page.GetByTestId("input-password");
    public ILocator LoginButton => Page.GetByTestId("btn-login");
    public ILocator ErrorMessageLocator => Page.GetByTestId("login-error-message");

    public async Task OpenAsync(string baseUrl)
    {
        await NavigateToAsync($"{baseUrl}/login");
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
