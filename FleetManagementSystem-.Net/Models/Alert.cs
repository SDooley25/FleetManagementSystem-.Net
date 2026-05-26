using System.Text.Json;
using FleetManagementSystem_.Net.Models.Enums;

namespace FleetManagementSystem_.Net.Models
{
    public class Alert
    {
        public string Message { get; set; }
        public AlertLevel Level { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Alert Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Alert>(json) ?? new Alert();
        }
        
        public static List<Alert> DeserializeAlerts(string json)
        {
            return JsonSerializer.Deserialize<List<Alert>>(json) ?? new List<Alert>();
        }
    }

    public static class AlertExtensions
    {
        public static string SerializeAlerts(this List<Alert> alerts)
        {
            return JsonSerializer.Serialize(alerts);
        }
        
    }
}
