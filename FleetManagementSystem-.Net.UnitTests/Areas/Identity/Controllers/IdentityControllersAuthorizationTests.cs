using FleetManagementSystem_.Net.Areas.Identity.Controllers;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Controllers;

[TestFixture]
public class IdentityControllersAuthorizationTests
{
    [Test]
    public void LoginController_HasAllowAnonymousAttribute()
    {
        var attribute = typeof(LoginController).GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true);

        Assert.That(attribute, Is.Not.Empty);
    }

    [Test]
    public void LogoutController_HasAuthorizeAttribute()
    {
        var attribute = typeof(LogoutController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Roles, Is.Null.Or.Empty);
    }

    [TestCase(typeof(RoleController))]
    [TestCase(typeof(UserController))]
    [TestCase(typeof(CompromisedPasswordController))]
    public void IdentitySectionControllers_HaveAuthorizeAttribute(Type controllerType)
    {
        var attribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Roles, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void RoleController_UsesSuperAdminRole()
    {
        var attribute = typeof(RoleController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();

        Assert.That(attribute.Roles, Is.EqualTo("SuperAdmin"));
    }

    [Test]
    public void UserController_UsesSuperAdminRole()
    {
        var attribute = typeof(UserController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();

        Assert.That(attribute.Roles, Is.EqualTo("SuperAdmin"));
    }

    [Test]
    public void CompromisedPasswordController_UsesSuperAdminRole()
    {
        var attribute = typeof(CompromisedPasswordController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();

        Assert.That(attribute.Roles, Is.EqualTo("SuperAdmin"));
    }

    [Test]
    public void RegisterController_UsesDisabledRole()
    {
        var controllerType = typeof(LoginController).Assembly.GetType("FleetManagementSystem_.Net.Areas.Identity.Controllers.RegisterController");
        Assert.That(controllerType, Is.Not.Null);

        var attribute = controllerType!.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();

        Assert.That(attribute.Roles, Is.EqualTo("__Disabled__"));
    }

    [Test]
    public void PasswordController_IndexAction_IsProtectedByControllerAuthorize()
    {
        var attribute = typeof(PasswordController).GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().Single();

        Assert.That(attribute, Is.Not.Null);
    }
}