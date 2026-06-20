using FleetManagementSystem_.Net.Areas.Identity.Models;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Models;

[TestFixture]
public class FMSUserTests
{
    private static List<ValidationResult> Validate(FMSUser model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    private static T? GetAttribute<T>(string propertyName) where T : Attribute
    {
        return typeof(FMSUser)
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<T>();
    }

    [TestCase(nameof(FMSUser.UserName))]
    [TestCase(nameof(FMSUser.Email))]
    public void Required_Properties_Have_RequiredAttribute(string propertyName)
    {
        Assert.That(GetAttribute<RequiredAttribute>(propertyName), Is.Not.Null);
    }

    [Test]
    public void Valid_Model_Passes_Validation()
    {
        var model = new FMSUser
        {
            UserName = "test.user",
            Email = "test.user@example.com"
        };

        var results = Validate(model);

        Assert.That(results, Is.Empty);
    }

    [TestCase(nameof(FMSUser.UserName))]
    [TestCase(nameof(FMSUser.Email))]
    public void Empty_String_Required_Fields_Fail_Validation(string propertyName)
    {
        var model = new FMSUser
        {
            UserName = "test.user",
            Email = "test.user@example.com"
        };

        typeof(FMSUser).GetProperty(propertyName)!.SetValue(model, string.Empty);

        var results = Validate(model);

        Assert.That(results.Any(r => r.MemberNames.Contains(propertyName)), Is.True);
    }
}