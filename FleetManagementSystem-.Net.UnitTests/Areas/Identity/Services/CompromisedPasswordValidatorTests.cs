using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Services;

[TestFixture]
public class CompromisedPasswordValidatorTests
{
    [Test]
    public async Task ValidateAsync_WhenPasswordIsCompromised_ReturnsFailedIdentityResult()
    {
        var service = new Mock<ICompromisedPasswordService>();
        service.Setup(s => s.IsCompromisedAsync("bad-password", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var logger = new Mock<ILogger<CompromisedPasswordValidator>>();
        var validator = new CompromisedPasswordValidator(service.Object, logger.Object);

        var result = await validator.ValidateAsync(new UserManager<FMSUser>(
            Mock.Of<IUserStore<FMSUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!),
            new FMSUser { UserName = "test.user" },
            "bad-password");

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors, Has.Exactly(1).Matches<IdentityError>(e => e.Code == "CompromisedPassword"));
    }

    [Test]
    public async Task ValidateAsync_WhenPasswordIsNotCompromised_ReturnsSuccess()
    {
        var service = new Mock<ICompromisedPasswordService>();
        service.Setup(s => s.IsCompromisedAsync("good-password", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var logger = new Mock<ILogger<CompromisedPasswordValidator>>();
        var validator = new CompromisedPasswordValidator(service.Object, logger.Object);

        var result = await validator.ValidateAsync(new UserManager<FMSUser>(
            Mock.Of<IUserStore<FMSUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!),
            new FMSUser { UserName = "test.user" },
            "good-password");

        Assert.That(result.Succeeded, Is.True);
    }

    [Test]
    public async Task ValidateAsync_WhenServiceThrows_ReturnsSuccess()
    {
        var service = new Mock<ICompromisedPasswordService>();
        service.Setup(s => s.IsCompromisedAsync("any-password", It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("error"));

        var logger = new Mock<ILogger<CompromisedPasswordValidator>>();
        var validator = new CompromisedPasswordValidator(service.Object, logger.Object);

        var result = await validator.ValidateAsync(new UserManager<FMSUser>(
            Mock.Of<IUserStore<FMSUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!),
            new FMSUser { UserName = "test.user" },
            "any-password");

        Assert.That(result.Succeeded, Is.True);
    }
}
