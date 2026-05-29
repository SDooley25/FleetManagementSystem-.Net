using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Models
{
    public class StorageSite
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Max Vehicle Capacity")]
        public int MaxVehicleCapacity { get; set; }

        [Required]
        [Display(Name = "Has Repair Station")]
        public bool HasRepairStation { get; set; }

        [Display(Name = "In Use Date")]
        [DataType(DataType.Date)]
        public DateTime? InUseDate { get; set; }

        [Display(Name = "Out Use Date")]
        [DataType(DataType.Date)]
        public DateTime? OutUseDate { get; set; }

        public StorageSite()
        {
            
        }

        public StorageSite(SqlDataReader dataReader, bool isList = false, bool withinVehicleStorage = false)
        {
            GetColumns(dataReader, isList, withinVehicleStorage);
        }

        public void GetColumns(SqlDataReader dataReader, bool isList = false, bool withinVehicleStorage = false)
        {
            if (!isList && !withinVehicleStorage)
            {
                dataReader.Read();
            }

            if (withinVehicleStorage)
            {
                Id = dataReader.Get<Guid>("StorageSiteId");
                Name = dataReader.Get<string>("StorageSiteName");
            }
            else
            {
                Id = dataReader.Get<Guid>("Id");
                Name = dataReader.Get<string>("Name");
            }

            Address = dataReader.Get<string>("Address");
            Postcode = dataReader.Get<string>("Postcode");
            MaxVehicleCapacity = dataReader.Get<int>("MaxVehicleCapacity");
            HasRepairStation = dataReader.Get<bool>("HasRepairStation");
            InUseDate = dataReader.Get<DateTime?>("InUseDate");
            OutUseDate = dataReader.Get<DateTime?>("OutUseDate");
        }

        public static List<StorageSite> GetListColumns(SqlDataReader reader)
        {
            var list = new List<StorageSite>();
            while (reader.Read())
            {
                list.Add(new StorageSite(reader, true));
            }
            return list;
        }
    }
}
