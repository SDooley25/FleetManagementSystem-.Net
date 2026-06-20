using FleetManagementSystem_.Net.Areas.VehicleStorage.ViewModels;
using FleetManagementSystem_.Net.Models.Enums;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Areas.VehicleStorage.ViewModels;

[TestFixture]
public class VehicleStorageEditViewModelTests
{
    private static List<ValidationResult> Validate(VehicleStorageEditViewModel model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    private static T? GetAttribute<T>(string propertyName) where T : Attribute
    {
        return typeof(VehicleStorageEditViewModel)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<T>();
    }

    [TestCase(nameof(VehicleStorageEditViewModel.VehicleId))]
    [TestCase(nameof(VehicleStorageEditViewModel.StorageSiteId))]
    [TestCase(nameof(VehicleStorageEditViewModel.StorageType))]
    [TestCase(nameof(VehicleStorageEditViewModel.StartDate))]
    public void Required_Properties_Have_RequiredAttribute(string propertyName)
    {
        Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
    }

    [TestCase(nameof(VehicleStorageEditViewModel.StartDate))]
    [TestCase(nameof(VehicleStorageEditViewModel.EndDate))]
    public void Date_Properties_Have_DateDataTypeAttribute(string propertyName)
    {
        var attribute = GetAttribute<DataTypeAttribute>(propertyName);

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.DataType, Is.EqualTo(DataType.Date));
    }

    [Test]
    public void Valid_Model_Passes_Validation()
    {
        var model = new VehicleStorageEditViewModel
        {
            VehicleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            StorageSiteId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            StorageType = StorageType.Main,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 2, 1),
            Note = "Test"
        };

        var results = Validate(model);

        Assert.That(results, Is.Empty);
    }
}