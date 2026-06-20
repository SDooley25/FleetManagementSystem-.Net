using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Services;
using FleetManagementSystem_.Net.Areas.Identity.Stores;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Services;

[TestFixture]
public class CompromisedPasswordServiceTests
{
    private static string Sha1Hex(string input)
    {
        using var sha1 = System.Security.Cryptography.SHA1.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha1.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    [Test]
    public async Task IsCompromisedAsync_EmptyPassword_ReturnsFalseAndDoesNotQueryRepository()
    {
        var repo = new Mock<ICompromisedPasswordRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<CompromisedPasswordService>>();
        var service = new CompromisedPasswordService(repo.Object, logger.Object);

        var result = await service.IsCompromisedAsync(string.Empty);

        Assert.That(result, Is.False);
        repo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task IsCompromisedAsync_WhenExactHashExists_ReturnsTrue()
    {
        var password = "P@ssw0rd123!";
        var hash = Sha1Hex(password);
        var prefix = hash[..5];
        var repo = new Mock<ICompromisedPasswordRepository>();
        repo.Setup(r => r.FindByHashPrefixAsync(prefix, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompromisedPassword>
            {
                new CompromisedPassword { PasswordHash = hash }
            });

        var logger = new Mock<ILogger<CompromisedPasswordService>>();
        var service = new CompromisedPasswordService(repo.Object, logger.Object);

        var result = await service.IsCompromisedAsync(password);

        Assert.That(result, Is.True);
        repo.Verify(r => r.FindByHashPrefixAsync(prefix, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task IsCompromisedAsync_WhenPrefixMatchesButHashDoesNot_ReturnsFalse()
    {
        var password = "P@ssw0rd123!";
        var hash = Sha1Hex(password);
        var prefix = hash[..5];
        var repo = new Mock<ICompromisedPasswordRepository>();
        repo.Setup(r => r.FindByHashPrefixAsync(prefix, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompromisedPassword>
            {
                new CompromisedPassword { PasswordHash = hash[..10] + "FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF" }
            });

        var logger = new Mock<ILogger<CompromisedPasswordService>>();
        var service = new CompromisedPasswordService(repo.Object, logger.Object);

        var result = await service.IsCompromisedAsync(password);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsCompromisedAsync_WhenRepositoryReturnsNoMatches_ReturnsFalse()
    {
        var password = "P@ssw0rd123!";
        var hash = Sha1Hex(password);
        var prefix = hash[..5];
        var repo = new Mock<ICompromisedPasswordRepository>();
        repo.Setup(r => r.FindByHashPrefixAsync(prefix, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CompromisedPassword>());

        var logger = new Mock<ILogger<CompromisedPasswordService>>();
        var service = new CompromisedPasswordService(repo.Object, logger.Object);

        var result = await service.IsCompromisedAsync(password);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task IsCompromisedAsync_WhenRepositoryThrows_ReturnsFalse()
    {
        var password = "P@ssw0rd123!";
        var hash = Sha1Hex(password);
        var prefix = hash[..5];
        var repo = new Mock<ICompromisedPasswordRepository>();
        repo.Setup(r => r.FindByHashPrefixAsync(prefix, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("db error"));

        var logger = new Mock<ILogger<CompromisedPasswordService>>();
        var service = new CompromisedPasswordService(repo.Object, logger.Object);

        var result = await service.IsCompromisedAsync(password);

        Assert.That(result, Is.False);
    }
}
