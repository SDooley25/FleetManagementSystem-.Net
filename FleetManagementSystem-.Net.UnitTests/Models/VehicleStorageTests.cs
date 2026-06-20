using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Models;

[TestFixture]
public class VehicleStorageTests
{
    private static List<ValidationResult> Validate(VehicleStorage model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    private static T? GetAttribute<T>(string propertyName) where T : Attribute
    {
        return typeof(VehicleStorage)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<T>();
    }

    [TestCase(nameof(VehicleStorage.Vehicle))]
    [TestCase(nameof(VehicleStorage.StorageSite))]
    [TestCase(nameof(VehicleStorage.StorageType))]
    [TestCase(nameof(VehicleStorage.StartDate))]
    public void Required_Properties_Have_RequiredAttribute(string propertyName)
    {
        Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
    }

    [TestCase(nameof(VehicleStorage.StartDate))]
    [TestCase(nameof(VehicleStorage.EndDate))]
    public void Date_Properties_Have_DateDataTypeAttribute(string propertyName)
    {
        var attribute = GetAttribute<DataTypeAttribute>(propertyName);

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.DataType, Is.EqualTo(DataType.Date));
    }

    [Test]
    public void Valid_Model_Passes_Validation()
    {
        var model = new VehicleStorage
        {
            Vehicle = new Vehicle
            {
                RegistrationNumber = "AB12CDE",
                Make = "Ford",
                Model = "Transit",
                FuelType = "Diesel",
                EngineSize = 2.0m,
                DateOfRegistration = new DateTime(2024, 1, 1)
            },
            StorageSite = new StorageSite
            {
                Name = "Depot 1",
                Address = "123 Test Street",
                Postcode = "AB12 3CD",
                MaxVehicleCapacity = 25,
                HasRepairStation = true
            },
            StorageType = StorageType.Main,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 2, 1),
            Note = "Test"
        };

        var results = Validate(model);

        Assert.That(results, Is.Empty);
    }
}