namespace Glorium.MedTech.MockServer;

public record MockPatient(
    string Id, 
    string FullName, 
    string Email, 
    string Phone, 
    string DateOfBirth, 
    string InsuranceNumber, 
    string StreetAddress, 
    string City, 
    string State, 
    string ZipCode);

public record MockEquipment(
    string Id, 
    string Name, 
    string Category, 
    string SerialNumber, 
    decimal DailyRate, 
    string Status);

public record CreateOrderDto(
    string PatientId, 
    List<string> EquipmentIds, 
    string ScheduledDeliveryDate, 
    string DeliveryNotes, 
    bool IsInsuranceCovered);

public record MockOrder(
    string Id, 
    string OrderNumber, 
    string PatientId, 
    string PatientName, 
    List<MockEquipment> EquipmentItems, 
    string ScheduledDeliveryDate, 
    decimal TotalAmount, 
    string Status, 
    DateTime CreatedAt);

public record MockInvoice(
    string Id, 
    string InvoiceNumber, 
    string OrderId, 
    decimal SubTotal, 
    decimal InsuranceCoverageAmount, 
    decimal PatientCopayAmount, 
    string Status);
