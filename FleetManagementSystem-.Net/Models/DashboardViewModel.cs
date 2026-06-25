using System;

namespace FleetManagementSystem_.Net.Models
{
    public class DashboardViewModel
    {
        public int UserCount { get; set; }
        public int VehicleCount { get; set; }
        public int StorageSiteCount { get; set; }
        public int TotalStorages { get; set; }
        public int TemporaryStorages { get; set; }
        public int ActiveStorages { get; set; }
        public int UpcomingMotCount { get; set; }

       
    }
}
