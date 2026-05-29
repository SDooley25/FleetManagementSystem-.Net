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

        // Additional stats can be added here (e.g., upcoming MOTs, vehicles due for service).
        // If a data source doesn't exist for a stat, leave default value and add a comment to implement.
    }
}
