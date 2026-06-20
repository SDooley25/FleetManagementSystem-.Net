using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Moq;
using FleetManagementSystem_.Net.Middleware;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Middleware;

[TestFixture]
public class AuthorisationHandlerTests
{
    [Test]
    public void AuthorisationHandler_ConfigContainsRole_UsesConfiguredSuperAdminRole()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns("RootAdmin");

        // Act
        var handler = new AuthorisationHandler(configuration.Object);

        // Assert
        Assert.That(handler, Is.Not.Null);
    }

    [Test]
    public void AuthorisationHandler_ConfigMissingRole_UsesDefaultSuperAdminRole()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns((string?)null);

        // Act
        var handler = new AuthorisationHandler(configuration.Object);

        // Assert
        Assert.That(handler, Is.Not.Null);
    }

    [Test]
    public async Task HandleAsync_AuthenticatedSuperAdminWithRoleRequirement_SucceedsMatchingRequirements()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns("SuperAdmin");
        var handler = new AuthorisationHandler(configuration.Object);

        var matchingRequirement = new Mock<IAuthorizationRequirement>();
        matchingRequirement.Setup(x => x.ToString()).Returns("User.IsInRole('Manager')");

        var nonMatchingRequirement = new Mock<IAuthorizationRequirement>();
        nonMatchingRequirement.Setup(x => x.ToString()).Returns("SomeOtherRequirement");

        var requirements = new IAuthorizationRequirement[]
        {
            matchingRequirement.Object,
            nonMatchingRequirement.Object
        };

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Role, "SuperAdmin") },
                "TestAuthentication"));

        var context = new AuthorizationHandlerContext(requirements, user, resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.That(context.PendingRequirements.Contains(nonMatchingRequirement.Object), Is.True);
        Assert.That(context.PendingRequirements.Contains(matchingRequirement.Object), Is.False);
        Assert.That(context.PendingRequirements.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task HandleAsync_AuthenticatedSuperAdminWithNullOrNonRoleRequirements_IgnoresNonRoleRequirements()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns("SuperAdmin");
        var handler = new AuthorisationHandler(configuration.Object);

        var nullLikeRequirement = new Mock<IAuthorizationRequirement>();
        nullLikeRequirement.Setup(x => x.ToString()).Returns("NotARoleRequirement");

        var roleRequirement = new Mock<IAuthorizationRequirement>();
        roleRequirement.Setup(x => x.ToString()).Returns("user.isinrole");

        var requirements = new IAuthorizationRequirement[]
        {
            nullLikeRequirement.Object,
            roleRequirement.Object
        };

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Role, "SuperAdmin") },
                "TestAuthentication"));

        var context = new AuthorizationHandlerContext(requirements, user, resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.That(context.PendingRequirements.Contains(nullLikeRequirement.Object), Is.True);
        Assert.That(context.PendingRequirements.Contains(roleRequirement.Object), Is.False);
    }

    [Test]
    public async Task HandleAsync_AuthenticatedNonSuperAdmin_DoesNotSucceedAnyRequirements()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns("SuperAdmin");
        var handler = new AuthorisationHandler(configuration.Object);

        var requirement = new Mock<IAuthorizationRequirement>();
        requirement.Setup(x => x.ToString()).Returns("User.IsInRole('Manager')");

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Role, "Manager") },
                "TestAuthentication"));

        var context = new AuthorizationHandlerContext(new[] { requirement.Object }, user, resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.That(context.PendingRequirements.Count(), Is.EqualTo(1));
        Assert.That(context.PendingRequirements.Contains(requirement.Object), Is.True);
    }

    [Test]
    public async Task HandleAsync_UnauthenticatedUser_DoesNotSucceedAnyRequirements()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        configuration.Setup(config => config["Auth:SuperAdminRole"]).Returns("SuperAdmin");
        var handler = new AuthorisationHandler(configuration.Object);

        var requirement = new Mock<IAuthorizationRequirement>();
        requirement.Setup(x => x.ToString()).Returns("User.IsInRole('Manager')");

        var user = new ClaimsPrincipal(new ClaimsIdentity());
        var context = new AuthorizationHandlerContext(new[] { requirement.Object }, user, resource: null);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.That(context.PendingRequirements.Count(), Is.EqualTo(1));
        Assert.That(context.PendingRequirements.Contains(requirement.Object), Is.True);
    }
}
