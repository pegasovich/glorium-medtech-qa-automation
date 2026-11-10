using System.Collections.Concurrent;

namespace Glorium.MedTech.MockServer;

public class MockServerApp
{
    private static readonly ConcurrentDictionary<string, WebApplication> _runningApps = new();

    public static async Task Main(string[] args)
    {
        var app = CreateApp(args, "http://127.0.0.1:5055");
        await app.RunAsync();
    }

    public static async Task<string> StartInMemoryAsync(string url = "http://127.0.0.1:5055")
    {
        if (_runningApps.ContainsKey(url)) return url;

        var app = CreateApp([], url);
        await app.StartAsync();
        _runningApps[url] = app;
        return url;
    }

    public static async Task StopInMemoryAsync(string? url = null)
    {
        if (!string.IsNullOrEmpty(url))
        {
            if (_runningApps.TryRemove(url, out var app))
            {
                await app.StopAsync();
                await app.DisposeAsync();
            }
        }
        else
        {
            foreach (var kvp in _runningApps)
            {
                if (_runningApps.TryRemove(kvp.Key, out var app))
                {
                    await app.StopAsync();
                    await app.DisposeAsync();
                }
            }
        }
    }

    public static WebApplication CreateApp(string[] args, string? url = null)
    {
        var builder = WebApplication.CreateBuilder(args);
        if (!string.IsNullOrEmpty(url))
        {
            builder.WebHost.UseUrls(url);
        }

        var app = builder.Build();

        var patients = new ConcurrentDictionary<string, MockPatient>();
        var equipment = new ConcurrentDictionary<string, MockEquipment>();
        var orders = new ConcurrentDictionary<string, MockOrder>();
        var invoices = new ConcurrentDictionary<string, MockInvoice>();

        SeedEquipment(equipment);

        // API Endpoints
        app.MapGet("/api/v1/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }));

        app.MapGet("/api/v1/patients", () => Results.Ok(patients.Values.ToList()));

        app.MapPost("/api/v1/patients", (MockPatient patient) =>
        {
            var id = string.IsNullOrEmpty(patient.Id) ? Guid.NewGuid().ToString("N")[..8] : patient.Id;
            var created = patient with { Id = id };
            patients[id] = created;
            return Results.Created($"/api/v1/patients/{id}", created);
        });

        app.MapGet("/api/v1/equipment", (string? category) =>
        {
            var query = equipment.Values.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(e => string.Equals(e.Category, category, StringComparison.OrdinalIgnoreCase));
            }
            return Results.Ok(query.ToList());
        });

        app.MapPost("/api/v1/equipment", (MockEquipment item) =>
        {
            var id = string.IsNullOrEmpty(item.Id) ? Guid.NewGuid().ToString("N")[..8] : item.Id;
            var created = item with { Id = id };
            equipment[id] = created;
            return Results.Created($"/api/v1/equipment/{id}", created);
        });

        app.MapGet("/api/v1/orders", () => Results.Ok(orders.Values.ToList()));

        app.MapGet("/api/v1/orders/{id}", (string id) =>
            orders.TryGetValue(id, out var order) ? Results.Ok(order) : Results.NotFound());

        app.MapPost("/api/v1/orders", (CreateOrderDto req) =>
        {
            if (!patients.TryGetValue(req.PatientId, out var patient))
            {
                return Results.BadRequest(new { error = "Patient not found" });
            }

            var orderId = Guid.NewGuid().ToString("N")[..8];
            var orderNum = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var selectedEquipment = req.EquipmentIds
                .Where(id => equipment.ContainsKey(id))
                .Select(id => equipment[id])
                .ToList();

            var totalAmount = selectedEquipment.Sum(e => e.DailyRate) * 30;

            var order = new MockOrder(
                Id: orderId,
                OrderNumber: orderNum,
                PatientId: patient.Id,
                PatientName: patient.FullName,
                EquipmentItems: selectedEquipment,
                ScheduledDeliveryDate: req.ScheduledDeliveryDate,
                TotalAmount: totalAmount,
                Status: "Created",
                CreatedAt: DateTime.UtcNow
            );

            orders[orderId] = order;

            var invoiceId = Guid.NewGuid().ToString("N")[..8];
            var coverageRate = req.IsInsuranceCovered ? 0.80m : 0.00m;
            var insuranceAmount = Math.Round(totalAmount * coverageRate, 2);
            var copayAmount = totalAmount - insuranceAmount;

            var invoice = new MockInvoice(
                Id: invoiceId,
                InvoiceNumber: $"INV-{new Random().Next(100000, 999999)}",
                OrderId: orderId,
                SubTotal: totalAmount,
                InsuranceCoverageAmount: insuranceAmount,
                PatientCopayAmount: copayAmount,
                Status: "Issued"
            );

            invoices[orderId] = invoice;

            return Results.Created($"/api/v1/orders/{orderId}", order);
        });

        app.MapGet("/api/v1/invoices/order/{orderId}", (string orderId) =>
            invoices.TryGetValue(orderId, out var inv) ? Results.Ok(inv) : Results.NotFound());

        // Web UI
        app.MapGet("/", () => Results.Redirect("/login"));
        app.MapGet("/login", () => Results.Content(HtmlTemplates.GetLoginPageHtml(), "text/html"));
        app.MapGet("/equipment", () => Results.Content(HtmlTemplates.GetEquipmentPageHtml(), "text/html"));
        app.MapGet("/orders/new", () => Results.Content(HtmlTemplates.GetCreateOrderPageHtml(), "text/html"));
        app.MapGet("/orders/{id}", (string id) =>
        {
            if (!orders.TryGetValue(id, out var ord)) return Results.NotFound("Order not found");
            invoices.TryGetValue(id, out var inv);
            return Results.Content(HtmlTemplates.GetOrderDetailsPageHtml(ord, inv), "text/html");
        });

        return app;
    }

    private static void SeedEquipment(ConcurrentDictionary<string, MockEquipment> dict)
    {
        var items = new[]
        {
            new MockEquipment("eq-1", "Philips Respironics EverFlo Oxygen Concentrator", "Respiratory", "SN-OXY-9021", 45.00m, "Available"),
            new MockEquipment("eq-2", "ResMed AirSense 11 AutoSet CPAP Machine", "Respiratory", "SN-CPAP-3321", 35.00m, "Available"),
            new MockEquipment("eq-3", "Drive Medical Cruiser III Light Wheelchair", "Mobility", "SN-WCH-7781", 20.00m, "Available"),
            new MockEquipment("eq-4", "Medline Full-Electric Bariatric Hospital Bed", "Mobility", "SN-BED-1002", 95.00m, "Available"),
            new MockEquipment("eq-5", "Nellcor PM10N Portable Pulse Oximeter", "Monitoring", "SN-MON-5544", 15.00m, "Available")
        };
        foreach (var item in items) dict[item.Id] = item;
    }
}
