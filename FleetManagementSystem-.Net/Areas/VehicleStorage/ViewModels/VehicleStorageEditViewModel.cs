using System.ComponentModel.DataAnnotations;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;

namespace FleetManagementSystem_.Net.Areas.VehicleStorage.ViewModels
{
    public class VehicleStorageEditViewModel
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Vehicle")]
        [Required]
        public Guid VehicleId { get; set; }

        [Display(Name = "Storage Site")]
        [Required]
        public Guid StorageSiteId { get; set; }

        [Display(Name = "Storage Type")]
        [Required]
        public StorageType StorageType { get; set; } = StorageType.Main;

        [Display(Name = "Start Date"), DataType(DataType.Date)]
        [Required]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date"), DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Note")]
        public string? Note { get; set; }

        // Select lists
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<StorageSite> Sites { get; set; } = new List<StorageSite>();
    }
}
