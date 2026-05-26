using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System;

namespace FleetManagementSystem_.Net.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string FuelType { get; set; } = string.Empty; // stored as full text
        public decimal EngineSize { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public DateTime? LastMotDate { get; set; }
        public int? LastMotMileage { get; set; }

        public Vehicle()
        {
        }

        public Vehicle(SqlDataReader dataReader, bool isList = false)
        {
            GetColumns(dataReader, isList);
        }

        public void GetColumns(SqlDataReader dataReader, bool isList = false)
        {
            if (!isList)
            {
                dataReader.Read();
            }
            Id = dataReader.Get<Guid>("Id");
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
