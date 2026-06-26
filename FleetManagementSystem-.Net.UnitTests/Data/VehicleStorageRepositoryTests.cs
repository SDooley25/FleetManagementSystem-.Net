using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace FleetManagementSystem_.Net.UnitTests.Data;

[TestFixture]
public class VehicleStorageRepositoryTests
{
    private static IConfiguration CreateConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AppSettings:DatabaseConnectionName"] = "Default",
                ["ConnectionStrings:Default"] = "Server=(local);Database=Test;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();
    }

    [Test]
    public async Task HasCapacityAvailableAsync_ReturnsFalse_WhenCountMeetsCapacity()
    {
        var repoMock = new Mock<VehicleStorageRepository>(CreateConfig(), Mock.Of<ILogger<VehicleStorageRepository>>()) { CallBase = true };
        repoMock.Setup(r => r.GetVehicleCountBySiteAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VehicleStorageCapacityCheck(3, 3, DateTime.Today));

        var item = new VehicleStorage
        {
            StorageSite = new StorageSite { Id = Guid.Parse("22222222-2222-2222-2222-222222222222") },
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 2)
        };

        var result = await repoMock.Object.HasCapacityAvailableAsync(item, CancellationToken.None);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasCapacityAvailableAsync_ReturnsTrue_WhenCapacityExistsForAllDates()
    {
        var repoMock = new Mock<VehicleStorageRepository>(CreateConfig(), Mock.Of<ILogger<VehicleStorageRepository>>()) { CallBase = true };
        repoMock.Setup(r => r.GetVehicleCountBySiteAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VehicleStorageCapacityCheck(2, 3, DateTime.Today));

        var item = new VehicleStorage
        {
            StorageSite = new StorageSite { Id = Guid.Parse("22222222-2222-2222-2222-222222222222") },
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 1, 2)
        };

        var result = await repoMock.Object.HasCapacityAvailableAsync(item, CancellationToken.None);

        Assert.That(result, Is.True);
    }
}