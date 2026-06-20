using System.Text.Json;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;
using NUnit.Framework;

namespace FleetManagementSystem_.Net.UnitTests.Models;

[TestFixture]
public class AlertTests
{
    [Test]
    public void Serialize_AlertWithMessageAndLevel_ReturnsExpectedJson()
    {
        // Arrange
        var alert = new Alert
        {
            Message = "Test message",
            Level = AlertLevel.Warning,
        };

        var expected = JsonSerializer.Serialize(alert);

        // Act
        var result = alert.Serialize();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void Deserialize_ValidJson_ReturnsExpectedAlert()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new Alert
        {
            Message = "Deserialize message",
            Level = AlertLevel.Information,
        });

        // Act
        var result = Alert.Deserialize(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Message, Is.EqualTo("Deserialize message"));
        Assert.That(result.Level, Is.EqualTo(AlertLevel.Information));
    }

    [Test]
    public void Deserialize_NullPropertyValues_ReturnsAlertWithDefaultValues()
    {
        // Arrange
        var json = "{}";

        // Act
        var result = Alert.Deserialize(json);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Message, Is.Null);
        Assert.That(result.Level, Is.EqualTo(default(AlertLevel)));
    }

    [Test]
    public void DeserializeAlerts_ValidJson_ReturnsExpectedAlerts()
    {
        // Arrange
        var expectedAlerts = new List<Alert>
        {
            new Alert { Message = "A", Level = AlertLevel.Success },
            new Alert { Message = "B", Level = AlertLevel.Error },
        };
        var json = JsonSerializer.Serialize(expectedAlerts);

        // Act
        var result = Alert.DeserializeAlerts(json);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Message, Is.EqualTo("A"));
        Assert.That(result[0].Level, Is.EqualTo(AlertLevel.Success));
        Assert.That(result[1].Message, Is.EqualTo("B"));
        Assert.That(result[1].Level, Is.EqualTo(AlertLevel.Error));
    }

    [Test]
    public void DeserializeAlerts_EmptyJsonArray_ReturnsEmptyList()
    {
        // Arrange
        var json = "[]";

        // Act
        var result = Alert.DeserializeAlerts(json);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void SerializeAlerts_ListWithAlerts_ReturnsExpectedJson()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            new Alert { Message = "First", Level = AlertLevel.Success },
            new Alert { Message = "Second", Level = AlertLevel.Warning },
        };

        var expected = JsonSerializer.Serialize(alerts);

        // Act
        var result = alerts.SerializeAlerts();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
