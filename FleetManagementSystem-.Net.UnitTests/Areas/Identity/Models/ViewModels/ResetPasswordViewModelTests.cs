using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Models.ViewModels;

[TestFixture]
public class ResetPasswordViewModelTests
{
    [Test]
    public void IsSamePassword_CurrentPasswordHashIsNull_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ResetPasswordViewModel
        {
            NewPassword = "P@ssw0rd123!",
        };

        // Act
        var result = viewModel.IsSamePassword(null);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsSamePassword_NewPasswordIsNullOrWhiteSpace_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ResetPasswordViewModel();

        // Act
        var resultWithNull = viewModel.IsSamePassword("AQAAAAEAACcQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        viewModel.NewPassword = "   ";
        var resultWithWhitespace = viewModel.IsSamePassword("AQAAAAEAACcQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");

        // Assert
        Assert.That(resultWithNull, Is.False);
        Assert.That(resultWithWhitespace, Is.False);
    }

    [Test]
    public void IsSamePassword_CurrentPasswordHashDoesNotStartWithExpectedPrefix_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ResetPasswordViewModel
        {
            NewPassword = "P@ssw0rd123!",
        };

        // Act
        var result = viewModel.IsSamePassword("NotAHashValue");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsSamePassword_CurrentPasswordHashMatchesNewPassword_ReturnsTrue()
    {
        // Arrange
        var password = "P@ssw0rd123!";
        var hasher = new PasswordHasher<FMSUser>();
        var viewModel = new ResetPasswordViewModel
        {
            NewPassword = password,
        };
        var hash = hasher.HashPassword(new FMSUser(), password);

        // Act
        var result = viewModel.IsSamePassword(hash);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsSamePassword_CurrentPasswordHashDoesNotMatchNewPassword_ReturnsFalse()
    {
        // Arrange
        var hasher = new PasswordHasher<FMSUser>();
        var viewModel = new ResetPasswordViewModel
        {
            NewPassword = "DifferentPassword123!",
        };
        var hash = hasher.HashPassword(new FMSUser(), "P@ssw0rd123!");

        // Act
        var result = viewModel.IsSamePassword(hash);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsSamePassword_CurrentPasswordHashIsMalformed_ReturnsFalse()
    {
        // Arrange
        var viewModel = new ResetPasswordViewModel
        {
            NewPassword = "P@ssw0rd123!",
        };

        // Act
        var result = viewModel.IsSamePassword("AQAAAAThisIsNotAValidPasswordHash");

        // Assert
        Assert.That(result, Is.False);
    }
}
