using Microsoft.Playwright;

namespace Glorium.Automation.Ui.Pages;

public class EquipmentCatalogPage(IPage page) : BasePage(page)
{
    public ILocator CatalogTitle => Page.GetByTestId("catalog-title");
    public ILocator CategorySelect => Page.GetByTestId("select-category");
    public ILocator EquipmentCount => Page.GetByTestId("equipment-count");
    public ILocator CreateOrderButton => Page.GetByTestId("btn-create-order");
    public ILocator EquipmentGrid => Page.GetByTestId("equipment-grid");

    public ILocator GetEquipmentCard(string equipmentId) =>
        Page.GetByTestId($"equipment-card-{equipmentId}");

    public async Task OpenAsync(string baseUrl)
    {
        await NavigateToAsync($"{baseUrl}/equipment");
    }

    public async Task FilterByCategoryAsync(string category)
    {
        await CategorySelect.SelectOptionAsync(new SelectOptionValue { Value = category });
    }

    public async Task ClickCreateOrderAsync()
    {
        await CreateOrderButton.ClickAsync();
    }
}
