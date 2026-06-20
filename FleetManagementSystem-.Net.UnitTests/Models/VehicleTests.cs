using FleetManagementSystem_.Net.Models;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Models;

[TestFixture]
public class VehicleTests
{
    private static List<ValidationResult> Validate(Vehicle model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    private static T? GetAttribute<T>(string propertyName) where T : Attribute
    {
        return typeof(Vehicle)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<T>();
    }

    [TestCase(nameof(Vehicle.RegistrationNumber))]
    [TestCase(nameof(Vehicle.Make))]
    [TestCase(nameof(Vehicle.Model))]
    [TestCase(nameof(Vehicle.FuelType))]
    [TestCase(nameof(Vehicle.EngineSize))]
    [TestCase(nameof(Vehicle.DateOfRegistration))]
    public void Required_Properties_Have_RequiredAttribute(string propertyName)
    {
        Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
    }

    [TestCase(nameof(Vehicle.DateOfRegistration))]
    [TestCase(nameof(Vehicle.LastMotDate))]
    public void Date_Properties_Have_DateDataTypeAttribute(string propertyName)
    {
        var attribute = GetAttribute<DataTypeAttribute>(propertyName);

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.DataType, Is.EqualTo(DataType.Date));
    }

    [Test]
    public void Valid_Model_Passes_Validation()
    {
        var model = new Vehicle
        {
            RegistrationNumber = "AB12CDE",
            Make = "Ford",
            Model = "Transit",
            FuelType = "Diesel",
            EngineSize = 2.0m,
            DateOfRegistration = new DateTime(2024, 1, 1),
            LastMotDate = new DateTime(2025, 1, 1),
            LastMotMileage = 12000
        };

        var results = Validate(model);

        Assert.That(results, Is.Empty);
    }

    [TestCase(nameof(Vehicle.RegistrationNumber))]
    [TestCase(nameof(Vehicle.Make))]
    [TestCase(nameof(Vehicle.Model))]
    [TestCase(nameof(Vehicle.FuelType))]
    public void Empty_String_Required_Fields_Fail_Validation(string propertyName)
    {
        var model = new Vehicle
        {
            RegistrationNumber = "AB12CDE",
            Make = "Ford",
            Model = "Transit",
            FuelType = "Diesel",
            EngineSize = 2.0m,
            DateOfRegistration = new DateTime(2024, 1, 1)
        };

        typeof(Vehicle).GetProperty(propertyName)!.SetValue(model, string.Empty);

        var results = Validate(model);

        Assert.That(results.Any(r => r.MemberNames.Contains(propertyName)), Is.True);
    }
}