using System.Text.Json.Serialization;

namespace Glorium.Automation.Api.Models;

public record PatientDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("fullName")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; init; } = string.Empty;

    [JsonPropertyName("dateOfBirth")]
    public string DateOfBirth { get; init; } = string.Empty;

    [JsonPropertyName("insuranceNumber")]
    public string InsuranceNumber { get; init; } = string.Empty;

    [JsonPropertyName("streetAddress")]
    public string StreetAddress { get; init; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("zipCode")]
    public string ZipCode { get; init; } = string.Empty;
}

public record EquipmentDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; init; } = string.Empty;

    [JsonPropertyName("serialNumber")]
    public string SerialNumber { get; init; } = string.Empty;

    [JsonPropertyName("dailyRate")]
    public decimal DailyRate { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = "Available";
}

public record CreateOrderRequest
{
    [JsonPropertyName("patientId")]
    public string PatientId { get; init; } = string.Empty;

    [JsonPropertyName("equipmentIds")]
    public List<string> EquipmentIds { get; init; } = [];

    [JsonPropertyName("scheduledDeliveryDate")]
    public string ScheduledDeliveryDate { get; init; } = string.Empty;

    [JsonPropertyName("deliveryNotes")]
    public string DeliveryNotes { get; init; } = string.Empty;

    [JsonPropertyName("isInsuranceCovered")]
    public bool IsInsuranceCovered { get; init; } = true;
}

public record OrderDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; init; } = string.Empty;

    [JsonPropertyName("patientId")]
    public string PatientId { get; init; } = string.Empty;

    [JsonPropertyName("patientName")]
    public string PatientName { get; init; } = string.Empty;

    [JsonPropertyName("equipmentItems")]
    public List<EquipmentDto> EquipmentItems { get; init; } = [];

    [JsonPropertyName("scheduledDeliveryDate")]
    public string ScheduledDeliveryDate { get; init; } = string.Empty;

    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = "Created";

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record InvoiceDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("invoiceNumber")]
    public string InvoiceNumber { get; init; } = string.Empty;

    [JsonPropertyName("orderId")]
    public string OrderId { get; init; } = string.Empty;

    [JsonPropertyName("subTotal")]
    public decimal SubTotal { get; init; }

    [JsonPropertyName("insuranceCoverageAmount")]
    public decimal InsuranceCoverageAmount { get; init; }

    [JsonPropertyName("patientCopayAmount")]
    public decimal PatientCopayAmount { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = "Issued";
}
