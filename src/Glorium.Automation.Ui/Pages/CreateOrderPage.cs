using Microsoft.Playwright;

namespace Glorium.Automation.Ui.Pages;

public class CreateOrderPage(IPage page) : BasePage(page)
{
    public ILocator PageTitle => Page.GetByTestId("page-title");
    public ILocator PatientSelect => Page.GetByTestId("select-patient");
    public ILocator EquipmentSelect => Page.GetByTestId("select-equipment");
    public ILocator DeliveryDateInput => Page.GetByTestId("input-delivery-date");
    public ILocator DeliveryNotesInput => Page.GetByTestId("input-delivery-notes");
    public ILocator InsuranceCoverageCheckbox => Page.GetByTestId("chk-insurance-coverage");
    public ILocator SubmitButton => Page.GetByTestId("btn-submit-order");

    public async Task OpenAsync(string baseUrl)
    {
        await NavigateToAsync($"{baseUrl}/orders/new");
    }

    public async Task SelectPatientAsync(string patientId)
    {
        await PatientSelect.SelectOptionAsync(new SelectOptionValue { Value = patientId });
    }

    public async Task SelectEquipmentAsync(string equipmentId)
    {
        await EquipmentSelect.SelectOptionAsync(new SelectOptionValue { Value = equipmentId });
    }

    public async Task SetDeliveryDateAsync(string dateYyyyMmDd)
    {
        await DeliveryDateInput.FillAsync(dateYyyyMmDd);
    }

    public async Task SetDeliveryNotesAsync(string notes)
    {
        await DeliveryNotesInput.FillAsync(notes);
    }

    public async Task SetInsuranceCoverageAsync(bool isCovered)
    {
        var isChecked = await InsuranceCoverageCheckbox.IsCheckedAsync();
        if (isChecked != isCovered)
        {
            await InsuranceCoverageCheckbox.ClickAsync();
        }
    }

    public async Task SubmitOrderAsync()
    {
        await SubmitButton.ClickAsync();
    }
}
