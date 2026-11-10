using Glorium.Automation.Ui.Pages;
using NUnit.Framework;

namespace Glorium.Tests.E2E;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("UI")]
[Category("Authentication")]
public class PortalAuthenticationTests : BaseE2ETest
{
    [Test]
    public async Task Login_WithValidClinicianCredentials_RedirectsToCatalog()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync(BaseUrl);

        // Act
        await loginPage.LoginAsync("dispatcher@gloriummed.com", "GloriumPass123!");

        // Assert
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*/equipment$"));
        var catalogPage = new EquipmentCatalogPage(Page);
        await Expect(catalogPage.CatalogTitle).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_WithInvalidPassword_ShowsErrorMessage()
    {
        // Arrange
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync(BaseUrl);

        // Act
        await loginPage.LoginAsync("dispatcher@gloriummed.com", "WrongPassword999");

        // Assert
        await Expect(loginPage.ErrorMessageLocator).ToBeVisibleAsync();
        await Expect(loginPage.ErrorMessageLocator).ToContainTextAsync("Invalid clinician credentials");
    }
}
