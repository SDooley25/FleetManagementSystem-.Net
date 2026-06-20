using FleetManagementSystem_.Net.Areas.Identity.Models;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Models;

[TestFixture]
public class FMSRoleTests
{
    private static List<ValidationResult> Validate(FMSRole model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    private static T? GetAttribute<T>(string propertyName) where T : Attribute
    {
        return typeof(FMSRole)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<T>();
    }

    [TestCase(nameof(FMSRole.Name))]
    [TestCase(nameof(FMSRole.Description))]
    public void Required_Properties_Have_RequiredAttribute(string propertyName)
    {
        Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
    }

    [Test]
    public void Valid_Model_Passes_Validation()
    {
        var model = new FMSRole
        {
            Name = "Admin",
            Description = "Administrators"
        };

        var results = Validate(model);

        Assert.That(results, Is.Empty);
    }

    [TestCase(nameof(FMSRole.Name))]
    [TestCase(nameof(FMSRole.Description))]
    public void Empty_String_Required_Fields_Fail_Validation(string propertyName)
    {
        var model = new FMSRole
        {
            Name = "Admin",
            Description = "Administrators"
        };

        typeof(FMSRole).GetProperty(propertyName)!.SetValue(model, string.Empty);

        var results = Validate(model);

        Assert.That(results.Any(r => r.MemberNames.Contains(propertyName)), Is.True);
    }
}