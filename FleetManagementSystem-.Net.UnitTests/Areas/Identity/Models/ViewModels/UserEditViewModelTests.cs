using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Areas.Identity.Models.ViewModels;

[TestFixture]
public class UserEditViewModelTests
{
    [Test]
    public void UserEditViewModel_DefaultConstructor_InitializesRolesAndLeavesPropertiesAtDefaults()
    {
        // Arrange & Act
        var viewModel = new UserEditViewModel();

        // Assert
        Assert.That(viewModel.Roles, Is.Not.Null);
        Assert.That(viewModel.Roles, Is.Empty);
        Assert.That(viewModel.Id, Is.Null);
        Assert.That(viewModel.UserName, Is.Null);
        Assert.That(viewModel.Password, Is.Null);
        Assert.That(viewModel.ConfirmPassword, Is.Null);
        Assert.That(viewModel.LockoutEnabled, Is.False);
        Assert.That(viewModel.LockoutEndLocal, Is.Null);
        Assert.That(viewModel.AccessFailedCount, Is.EqualTo(0));
        Assert.That(viewModel.SelectedRoles, Is.Null);
    }

    [Test]
    public void UserEditViewModel_ParameterizedConstructor_SetsScalarPropertiesAndMapsSelectedRoles()
    {
        // Arrange
        var user = new FMSUser
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "test.user",
            LockoutEnabled = true,
            LockoutEnd = new DateTimeOffset(2024, 04, 30, 15, 45, 00, TimeSpan.Zero),
            AccessFailedCount = 3,
        };

        var allRoles = new List<FMSRole>
        {
            new FMSRole
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Admin",
                Description = "Administrators",
            },
            new FMSRole
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Viewer",
                Description = "Read-only users",
            },
        };

        IList<string> selectedRoles = new List<string> { "Admin" };
        var expectedLockoutEndLocal = user.LockoutEnd.Value.ToLocalTime().ToString("yyyy-MM-dd'T'HH:mm");

        // Act
        var viewModel = new UserEditViewModel(user, allRoles, selectedRoles);

        // Assert
        Assert.That(viewModel.Id, Is.EqualTo(user.Id.ToString()));
        Assert.That(viewModel.UserName, Is.EqualTo(user.UserName));
        Assert.That(viewModel.LockoutEnabled, Is.EqualTo(user.LockoutEnabled));
        Assert.That(viewModel.LockoutEndLocal, Is.EqualTo(expectedLockoutEndLocal));
        Assert.That(viewModel.AccessFailedCount, Is.EqualTo(user.AccessFailedCount));
        Assert.That(viewModel.Roles, Has.Count.EqualTo(2));

        var adminRole = viewModel.Roles[0];
        Assert.That(adminRole.Id, Is.EqualTo(allRoles[0].Id));
        Assert.That(adminRole.Name, Is.EqualTo(allRoles[0].Name));
        Assert.That(adminRole.Description, Is.EqualTo(allRoles[0].Description));
        Assert.That(adminRole.IsSelected, Is.True);

        var viewerRole = viewModel.Roles[1];
        Assert.That(viewerRole.Id, Is.EqualTo(allRoles[1].Id));
        Assert.That(viewerRole.Name, Is.EqualTo(allRoles[1].Name));
        Assert.That(viewerRole.Description, Is.EqualTo(allRoles[1].Description));
        Assert.That(viewerRole.IsSelected, Is.False);
    }

    [Test]
    public void UserEditViewModel_ParameterizedConstructor_NullLockoutEndAndBlankRoleNames_DoNotSelectRoles()
    {
        // Arrange
        var user = new FMSUser
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            UserName = "another.user",
            LockoutEnabled = false,
            LockoutEnd = null,
            AccessFailedCount = 0,
        };

        var allRoles = new List<FMSRole>
        {
            new FMSRole
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = string.Empty,
                Description = "Blank role name",
            },
            new FMSRole
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Name = null,
                Description = "Null role name",
            },
        };

        IList<string> selectedRoles = new List<string> { string.Empty };

        // Act
        var viewModel = new UserEditViewModel(user, allRoles, selectedRoles);

        // Assert
        Assert.That(viewModel.LockoutEndLocal, Is.Null);
        Assert.That(viewModel.Roles, Has.Count.EqualTo(2));
        Assert.That(viewModel.Roles[0].IsSelected, Is.False);
        Assert.That(viewModel.Roles[1].IsSelected, Is.False);
        Assert.That(viewModel.Roles[0].Description, Is.EqualTo(allRoles[0].Description));
        Assert.That(viewModel.Roles[1].Description, Is.EqualTo(allRoles[1].Description));
    }
}
