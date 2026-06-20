using FleetManagementSystem_.Net.Areas.VehicleStorage.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System.Reflection;

namespace FleetManagementSystem_.Net.UnitTests.Areas.VehicleStorage.Controllers;

[TestFixture]
public class VehicleStorageControllersAuthorizationTests
{
    [TestCase(typeof(StorageController), "VehicleStorage.Edit")]
    [TestCase(typeof(StorageSiteController), "VehicleStorage.Edit")]
    [TestCase(typeof(VehicleController), "VehicleStorage.Edit")]
    public void VehicleStorageSectionControllers_HaveEditRoleAuthorizeAttribute(Type controllerType, string expectedRole)
    {
        var attribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();

        Assert.That(attribute, Is.Not.Null);
        Assert.That(attribute!.Roles, Is.EqualTo(expectedRole));
    }

    [TestCase(typeof(StorageController), "Delete")]
    [TestCase(typeof(StorageSiteController), "Delete")]
    [TestCase(typeof(VehicleController), "Delete")]
    public void DeleteActions_HaveAdminRoleAuthorizeAttribute(Type controllerType, string actionName)
    {
        var method = controllerType.GetMethod(actionName, BindingFlags.Instance | BindingFlags.Public)!;
        var authorize = method.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true).Cast<AuthorizeAttribute>().SingleOrDefault();
        var httpPost = method.GetCustomAttributes(typeof(HttpPostAttribute), inherit: true).SingleOrDefault();

        Assert.That(httpPost, Is.Not.Null);
        Assert.That(authorize, Is.Not.Null);
        Assert.That(authorize!.Roles, Is.EqualTo("VehicleStorage.Admin"));
        Assert.That(authorize.Roles, Does.EndWith(".Admin"));
    }
}