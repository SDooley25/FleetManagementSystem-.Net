using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        [Display(Name = "Registration Number")]
        [Required]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Display(Name = "Make")]
        [Required]
        public string Make { get; set; } = string.Empty;

        [Display(Name = "Model")]
        [Required]
        public string Model { get; set; } = string.Empty;

        [Display(Name = "Fuel Type")]
        [Required]
        public string FuelType { get; set; } = string.Empty; // stored as full text

        [Display(Name = "Engine Size")]
        [Required]
        public decimal EngineSize { get; set; }

        [Display(Name = "Date Of Registration")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfRegistration { get; set; }

        [Display(Name = "Last MOT Date")]
        [DataType(DataType.Date)]
        public DateTime? LastMotDate { get; set; }

        [Display(Name = "Last MOT Mileage")]
        public int? LastMotMileage { get; set; }

        public Vehicle()
        {
        }

        public Vehicle(SqlDataReader dataReader, bool isList = false, bool withinVehicleStorage = false)
        {
            GetColumns(dataReader, isList, withinVehicleStorage);
        }

        public void GetColumns(SqlDataReader dataReader, bool isList = false, bool withinVehicleStorage = false)
        {
            // when reading as part of vehicle storage view avoid advancing the reader and use different column names
            if (!isList && !withinVehicleStorage)
            {
                dataReader.Read();
            }

            if (withinVehicleStorage)
            {
                Id = dataReader.Get<Guid>("VehicleId");
            }
            else
            {
                Id = dataReader.Get<Guid>("Id");
            }

            RegistrationNumber = dataReader.Get<string>("RegistrationNumber");
            Make = dataReader.Get<string>("Make");
            Model = dataReader.Get<string>("Model");
            FuelType = dataReader.Get<string>("FuelType");
            EngineSize = dataReader.Get<decimal>("EngineSize");
            DateOfRegistration = dataReader.Get<DateTime>("DateOfRegistration");
            LastMotDate = dataReader.Get<DateTime?>("LastMotDate");
            LastMotMileage = dataReader.Get<int?>("LastMotMileage");
        }

        public static List<Vehicle> GetListColumns(SqlDataReader reader)
        {
            var list = new List<Vehicle>();
            while (reader.Read())
            {
                list.Add(new Vehicle(reader, true));
            }
            return list;
        }
    }
}
