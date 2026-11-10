using System.Text.Json;
using Glorium.Automation.Api.Models;
using Glorium.Automation.Core.DataGenerators;
using Glorium.Automation.Core.Logging;
using Microsoft.Playwright;

namespace Glorium.Automation.Api.Clients;

public class MedTechApiClient(IAPIRequestContext requestContext, string baseUrl)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IAPIResponse> GetHealthAsync()
    {
        TestLogger.Info($"[API] GET {baseUrl}/health");
        return await requestContext.GetAsync($"{baseUrl}/health");
    }

    public async Task<PatientDto> CreatePatientAsync(PatientFakerModel patient)
    {
        TestLogger.Info($"[API] POST {baseUrl}/patients (Creating patient {patient.FullName})");
        var response = await requestContext.PostAsync($"{baseUrl}/patients", new APIRequestContextOptions
        {
            DataObject = patient
        });

        if (!response.Ok)
        {
            var err = await response.TextAsync();
            throw new InvalidOperationException($"Failed to create patient: {response.Status} - {err}");
        }

        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<PatientDto>(json, JsonOptions)!;
    }

    public async Task<List<PatientDto>> GetPatientsAsync()
    {
        TestLogger.Info($"[API] GET {baseUrl}/patients");
        var response = await requestContext.GetAsync($"{baseUrl}/patients");
        response.Ok.ShouldBeTrue($"Failed GET /patients: {response.Status}");
        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<List<PatientDto>>(json, JsonOptions) ?? [];
    }

    public async Task<EquipmentDto> CreateEquipmentAsync(EquipmentFakerModel equipment)
    {
        TestLogger.Info($"[API] POST {baseUrl}/equipment (Creating {equipment.Name})");
        var response = await requestContext.PostAsync($"{baseUrl}/equipment", new APIRequestContextOptions
        {
            DataObject = equipment
        });

        if (!response.Ok)
        {
            var err = await response.TextAsync();
            throw new InvalidOperationException($"Failed to create equipment: {response.Status} - {err}");
        }

        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<EquipmentDto>(json, JsonOptions)!;
    }

    public async Task<List<EquipmentDto>> GetEquipmentAsync(string? category = null)
    {
        var url = string.IsNullOrWhiteSpace(category) ? $"{baseUrl}/equipment" : $"{baseUrl}/equipment?category={Uri.EscapeDataString(category)}";
        TestLogger.Info($"[API] GET {url}");
        var response = await requestContext.GetAsync(url);
        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<List<EquipmentDto>>(json, JsonOptions) ?? [];
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderRequest orderRequest)
    {
        TestLogger.Info($"[API] POST {baseUrl}/orders (PatientId: {orderRequest.PatientId})");
        var response = await requestContext.PostAsync($"{baseUrl}/orders", new APIRequestContextOptions
        {
            DataObject = orderRequest
        });

        if (!response.Ok)
        {
            var err = await response.TextAsync();
            throw new InvalidOperationException($"Failed to create order: {response.Status} - {err}");
        }

        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<OrderDto>(json, JsonOptions)!;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(string orderId)
    {
        TestLogger.Info($"[API] GET {baseUrl}/orders/{orderId}");
        var response = await requestContext.GetAsync($"{baseUrl}/orders/{orderId}");
        if (response.Status == 404) return null;
        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<OrderDto>(json, JsonOptions);
    }

    public async Task<InvoiceDto?> GetInvoiceByOrderIdAsync(string orderId)
    {
        TestLogger.Info($"[API] GET {baseUrl}/invoices/order/{orderId}");
        var response = await requestContext.GetAsync($"{baseUrl}/invoices/order/{orderId}");
        if (response.Status == 404) return null;
        var json = await response.TextAsync();
        return JsonSerializer.Deserialize<InvoiceDto>(json, JsonOptions);
    }
}

internal static class ResponseAssertionExtensions
{
    public static void ShouldBeTrue(this bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
