using FluentAssertions;
using Glorium.Automation.Core.DataGenerators;
using NUnit.Framework;

namespace Glorium.Tests.Api;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
[Category("API")]
[Category("Patients")]
public class PatientApiTests : BaseApiTestFixture
{
    [Test]
    public async Task HealthCheck_ReturnsHealthyStatus()
    {
        // Act
        var response = await ApiClient.GetHealthAsync();

        // Assert
        response.Ok.Should().BeTrue();
        response.Status.Should().Be(200);
    }

    [Test]
    public async Task CreatePatient_WithValidDemographics_ReturnsCreatedPatient()
    {
        // Arrange
        var fakePatient = MedicalDataFaker.PatientGenerator.Generate();

        // Act
        var created = await ApiClient.CreatePatientAsync(fakePatient);

        // Assert
        created.Should().NotBeNull();
        created.Id.Should().NotBeNullOrWhiteSpace();
        created.FullName.Should().Be(fakePatient.FullName);
        created.InsuranceNumber.Should().Be(fakePatient.InsuranceNumber);
        created.City.Should().Be(fakePatient.City);

        // Verify retrieval via GET
        var all = await ApiClient.GetPatientsAsync();
        all.Should().Contain(p => p.Id == created.Id && p.FullName == fakePatient.FullName);
    }
}
