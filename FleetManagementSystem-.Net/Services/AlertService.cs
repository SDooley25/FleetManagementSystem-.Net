using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;

namespace FleetManagementSystem_.Net.Services
{
    public interface IAlertService
    {
        void AddAlert(string message, AlertLevel level);
        List<Alert> GetAlerts();
        void ClearAlerts();
    }

    public class AlertService : IAlertService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _sessionKey = "Alerts";

        public AlertService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void AddAlert(string message, AlertLevel level)
        {
            var alerts = GetAlerts();
            alerts.Add(new Alert { Message = message, Level = level });
            SetAlerts(alerts);
        }
        
        public void ClearAlerts()
        {
           SetAlerts(new List<Alert>());
        }

        public void SetAlerts(List<Alert> alerts)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.SetString(_sessionKey, alerts.SerializeAlerts());
        }

        public List<Alert> GetAlerts()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var json = httpContext.Session.GetString(_sessionKey);
            if (!string.IsNullOrWhiteSpace(json))
            {
                return Alert.DeserializeAlerts(json);
            }
            return new List<Alert>();
        }
    }
}
