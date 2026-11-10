using FluentAssertions;
using Glorium.Automation.Core.DataGenerators;
using Glorium.Automation.Ui.Pages;
using NUnit.Framework;

namespace Glorium.Tests.E2E;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("UI")]
[Category("E2E")]
[Category("Orders")]
public class MedicalEquipmentOrderFlowTests : BaseE2ETest
{
    [Test]
    public async Task CreateOrder_HybridFlow_DispatchesEquipmentAndGeneratesInvoice()
    {
        // ---------------------------------------------------------------------
        // 1. Arrange (API Preconditions: Fast & isolated test data setup)
        // ---------------------------------------------------------------------
        var fakePatient = MedicalDataFaker.PatientGenerator.Generate();
        var patient = await ApiClient.CreatePatientAsync(fakePatient);

        var fakeEquipment = MedicalDataFaker.EquipmentGenerator.Generate();
        fakeEquipment.Category = "Respiratory";
        var equipment = await ApiClient.CreateEquipmentAsync(fakeEquipment);

        // ---------------------------------------------------------------------
        // 2. Act (UI Flow): Login -> View Catalog -> Create Order
        // ---------------------------------------------------------------------
        var loginPage = new LoginPage(Page);
        await loginPage.OpenAsync(BaseUrl);
        await loginPage.LoginAsync("dispatcher@gloriummed.com", "GloriumPass123!");

        var catalogPage = new EquipmentCatalogPage(Page);
        await Expect(catalogPage.CatalogTitle).ToBeVisibleAsync();
        
        // Filter by category and verify new equipment is listed
        await catalogPage.FilterByCategoryAsync("Respiratory");
        var equipmentCard = catalogPage.GetEquipmentCard(equipment.Id);
        await Expect(equipmentCard).ToBeVisibleAsync();

        // Navigate to Order scheduling
        await catalogPage.ClickCreateOrderAsync();
        var orderPage = new CreateOrderPage(Page);
        await Expect(orderPage.PageTitle).ToBeVisibleAsync();

        // Fill scheduling form
        var scheduledDate = DateTime.UtcNow.AddDays(3).ToString("yyyy-MM-dd");
        await orderPage.SelectPatientAsync(patient.Id);
        await orderPage.SelectEquipmentAsync(equipment.Id);
        await orderPage.SetDeliveryDateAsync(scheduledDate);
        await orderPage.SetDeliveryNotesAsync("Leave at nursing station, urgent oxygen needed.");
        await orderPage.SetInsuranceCoverageAsync(true);

        // Submit
        await orderPage.SubmitOrderAsync();

        // ---------------------------------------------------------------------
        // 3. Assert (UI Verification): Order Confirmation & Auto-Generated Invoice
        // ---------------------------------------------------------------------
        var detailsPage = new OrderDetailsPage(Page);
        await Expect(detailsPage.OrderHeader).ToBeVisibleAsync();
        await Expect(detailsPage.OrderStatus).ToHaveTextAsync("Created");
        await Expect(detailsPage.PatientName).ToContainTextAsync(patient.FullName);
        await Expect(detailsPage.DeliveryDate).ToHaveTextAsync(scheduledDate);
        await Expect(detailsPage.OrderedItems).ToContainTextAsync(equipment.Name);

        // Verify MedTech SaaS Invoicing calculation
        await Expect(detailsPage.InvoiceSection).ToBeVisibleAsync();
        
        var expectedSubtotal = equipment.DailyRate * 30;
        var expectedInsuranceCovered = Math.Round(expectedSubtotal * 0.80m, 2);
        var expectedCopay = expectedSubtotal - expectedInsuranceCovered;

        await Expect(detailsPage.InvoiceSubtotal).ToContainTextAsync($"${expectedSubtotal:F2}");
        await Expect(detailsPage.InsuranceCoverage).ToContainTextAsync($"-${expectedInsuranceCovered:F2}");
        await Expect(detailsPage.PatientCopay).ToContainTextAsync($"${expectedCopay:F2}");
    }
}
