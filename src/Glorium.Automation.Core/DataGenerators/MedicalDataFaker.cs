using Bogus;

namespace Glorium.Automation.Core.DataGenerators;

public static class MedicalDataFaker
{
    private static readonly string[] EquipmentCategories = ["Respiratory", "Mobility", "Monitoring", "Infusion", "Pediatric"];
    
    private static readonly string[] EquipmentTemplates = 
    [
        "Philips Respironics EverFlo Oxygen Concentrator",
        "ResMed AirSense 11 AutoSet CPAP",
        "Drive Medical Cruiser III Light Wheelchair",
        "Medline Full-Electric Bariatric Hospital Bed",
        "Baxter Colleague CX3 Volumetric Infusion Pump",
        "Nellcor PM10N Portable Pulse Oximeter",
        "Invacare Platinum 10 Continuous Oxygen Machine"
    ];

    public static Faker<PatientFakerModel> PatientGenerator => new Faker<PatientFakerModel>("en_US")
        .RuleFor(p => p.FullName, f => f.Name.FullName())
        .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FullName))
        .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("###-###-####"))
        .RuleFor(p => p.DateOfBirth, f => f.Date.Past(60, DateTime.UtcNow.AddYears(-18)).ToString("yyyy-MM-dd"))
        .RuleFor(p => p.InsuranceNumber, f => $"MED-{f.Random.AlphaNumeric(8).ToUpperInvariant()}")
        .RuleFor(p => p.StreetAddress, f => f.Address.StreetAddress())
        .RuleFor(p => p.City, f => f.Address.City())
        .RuleFor(p => p.State, f => f.Address.StateAbbr())
        .RuleFor(p => p.ZipCode, f => f.Address.ZipCode("#####"));

    public static Faker<EquipmentFakerModel> EquipmentGenerator => new Faker<EquipmentFakerModel>("en_US")
        .RuleFor(e => e.Name, f => f.PickRandom(EquipmentTemplates))
        .RuleFor(e => e.Category, f => f.PickRandom(EquipmentCategories))
        .RuleFor(e => e.SerialNumber, f => $"SN-{f.Random.AlphaNumeric(6).ToUpperInvariant()}-{f.Random.Number(100, 999)}")
        .RuleFor(e => e.DailyRate, f => Math.Round(f.Random.Decimal(15.00m, 120.00m), 2))
        .RuleFor(e => e.Status, _ => "Available");
}

public class PatientFakerModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string InsuranceNumber { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}

public class EquipmentFakerModel
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public decimal DailyRate { get; set; } = 45.00m;
    public string Status { get; set; } = "Available";
}
