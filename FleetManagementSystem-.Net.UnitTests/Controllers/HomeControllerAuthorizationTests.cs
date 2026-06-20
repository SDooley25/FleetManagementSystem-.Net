using FleetManagementSystem_.Net.Controllers;
using Microsoft.AspNetCore.Authorization;
using NUnit.Framework;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Controllers;

[TestFixture]
public class HomeControllerAuthorizationTests
{
    [Test]
    public void IndexAction_HasAuthorizeAttribute()
    {
        var method = typeof(HomeController).GetMethod(nameof(HomeController.Index), BindingFlags.Instance | BindingFlags.Public)!;
        var attribute = method.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Roles, Is.Null.Or.Empty);
    }

    [Test]
    public void AccessDeniedAction_HasAllowAnonymousAttribute()
    {
        var method = typeof(HomeController).GetMethod(nameof(HomeController.AccessDenied), BindingFlags.Instance | BindingFlags.Public)!;
        var attribute = method.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
    }

    [Test]
    public void PrivacyAction_IsProtectedByDisabledRole()
    {
        var method = typeof(HomeController).GetMethod("Privacy", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var attribute = method.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Roles, Is.EqualTo("__Disabled__"));
    }
}