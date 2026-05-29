using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System;

namespace FleetManagementSystem_.Net.Models
{
    using FleetManagementSystem_.Net.Models.Enums;
    using System.ComponentModel.DataAnnotations;

    

    public class VehicleStorage
    {
        [Display(Name = "Id")]
        public Guid Id { get; set; }

        [Display(Name = "Vehicle")]
        [Required]
        public Vehicle Vehicle { get; set; } = new Vehicle();

        [Display(Name = "Storage Site")]
        [Required]
        public StorageSite StorageSite { get; set; } = new StorageSite();

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

        public VehicleStorage()
        {
        }

        public VehicleStorage(SqlDataReader reader, bool isList = false)
        {
            GetColumns(reader, isList);
        }

        public void GetColumns(SqlDataReader reader, bool isList = false)
        {
            if (!isList)
            {
                reader.Read();
            }

            // id column may be named VehicleStorageId or Id depending on view
            try
            {
                Id = reader.Get<Guid>("VehicleStorageId");
            }
            catch
            {
                Id = reader.Get<Guid>("Id");
            }

            // read storage type as string and parse to enum; default to Main on parse failure
            var sType = reader.Get<string>("StorageType");
            if (!Enum.TryParse<StorageType>(sType, true, out var parsed))
            {
                parsed = StorageType.Main;
            }
            StorageType = parsed;
            StartDate = reader.Get<DateTime>("StartDate");
            EndDate = reader.Get<DateTime?>("EndDate");
            Note = reader.Get<string>("Note");

            // vehicle and site columns are present in the view; read using existing models without advancing reader
            Vehicle = new Vehicle(reader, isList, true);
            StorageSite = new StorageSite(reader, isList, true);
        }

        public static List<VehicleStorage> GetListColumns(SqlDataReader reader)
        {
            var list = new List<VehicleStorage>();
            while (reader.Read())
            {
                list.Add(new VehicleStorage(reader, true));
            }
            return list;
        }
    }
}
