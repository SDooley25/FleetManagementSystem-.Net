using FleetManagementSystem_.Net.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Models
{
    [TestFixture]
    public class StorageSiteTests
    {
        private static List<ValidationResult> Validate(StorageSite model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }

        private static T? GetAttribute<T>(string propertyName) where T : Attribute
        {
            return typeof(StorageSite)
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
                .GetCustomAttribute<T>();
        }

        [TestCase(nameof(StorageSite.Name))]
        [TestCase(nameof(StorageSite.Address))]
        [TestCase(nameof(StorageSite.Postcode))]
        [TestCase(nameof(StorageSite.MaxVehicleCapacity))]
        [TestCase(nameof(StorageSite.HasRepairStation))]
        public void Required_Properties_Have_RequiredAttribute(string propertyName)
        {
            Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
        }

        [TestCase(nameof(StorageSite.InUseDate))]
        [TestCase(nameof(StorageSite.OutUseDate))]
        public void Date_Properties_Have_DateDataTypeAttribute(string propertyName)
        {
            var attribute = GetAttribute<DataTypeAttribute>(propertyName);

            Assert.That(attribute, Is.Not.Null);
            Assert.That(attribute!.DataType, Is.EqualTo(DataType.Date));
        }

        [Test]
        public void Valid_Model_Passes_Validation()
        {
            var model = new StorageSite
            {
                Name = "Depot 1",
                Address = "123 Test Street",
                Postcode = "AB12 3CD",
                MaxVehicleCapacity = 25,
                HasRepairStation = true,
                InUseDate = DateTime.Today,
                OutUseDate = DateTime.Today.AddDays(30)
            };

            var results = Validate(model);

            Assert.That(results, Is.Empty);
        }

        [TestCase(nameof(StorageSite.Name))]
        [TestCase(nameof(StorageSite.Address))]
        [TestCase(nameof(StorageSite.Postcode))]
        public void Empty_String_Required_Fields_Fail_Validation(string propertyName)
        {
            var model = new StorageSite
            {
                Name = "Depot 1",
                Address = "123 Test Street",
                Postcode = "AB12 3CD",
                MaxVehicleCapacity = 25,
                HasRepairStation = true
            };

            typeof(StorageSite).GetProperty(propertyName)!.SetValue(model, string.Empty);

            var results = Validate(model);

            Assert.That(results.Any(r => r.MemberNames.Contains(propertyName)), Is.True);
        }
    }
}
