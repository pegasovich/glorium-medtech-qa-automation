using FluentAssertions;
using Glorium.Automation.Api.Models;
using Glorium.Automation.Core.DataGenerators;
using NUnit.Framework;

namespace Glorium.Tests.Api;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("API")]
[Category("Orders")]
public class EquipmentAndOrderApiTests : BaseApiTestFixture
{
    [Test]
    public async Task GetEquipment_WithCategoryFilter_ReturnsMatchingCategoryOnly()
    {
        // Act
        var respiratoryItems = await ApiClient.GetEquipmentAsync("Respiratory");

        // Assert
        respiratoryItems.Should().NotBeEmpty();
        respiratoryItems.Should().OnlyContain(e => e.Category == "Respiratory");
    }

    [Test]
    public async Task CreateOrder_WithInsuranceCoverage_CalculatesCopayCorrectly()
    {
        // Arrange
        var fakePatient = MedicalDataFaker.PatientGenerator.Generate();
        var patient = await ApiClient.CreatePatientAsync(fakePatient);

        var equipment = await ApiClient.GetEquipmentAsync();
        var selectedItem = equipment.First();

        var orderRequest = new CreateOrderRequest
        {
            PatientId = patient.Id,
            EquipmentIds = [selectedItem.Id],
            ScheduledDeliveryDate = DateTime.UtcNow.AddDays(2).ToString("yyyy-MM-dd"),
            DeliveryNotes = "Standard medical courier drop-off",
            IsInsuranceCovered = true
        };

        // Act
        var order = await ApiClient.CreateOrderAsync(orderRequest);

        // Assert Order
        order.Should().NotBeNull();
        order.PatientId.Should().Be(patient.Id);
        order.PatientName.Should().Be(patient.FullName);
        order.Status.Should().Be("Created");
        order.TotalAmount.Should().Be(selectedItem.DailyRate * 30);

        // Assert Invoice
        var invoice = await ApiClient.GetInvoiceByOrderIdAsync(order.Id);
        invoice.Should().NotBeNull();
        invoice!.OrderId.Should().Be(order.Id);
        invoice.SubTotal.Should().Be(order.TotalAmount);
        
        // 80% coverage check
        var expectedInsurance = Math.Round(order.TotalAmount * 0.80m, 2);
        invoice.InsuranceCoverageAmount.Should().Be(expectedInsurance);
        invoice.PatientCopayAmount.Should().Be(order.TotalAmount - expectedInsurance);
    }
}
