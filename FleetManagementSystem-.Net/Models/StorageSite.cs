using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.CodeDom;
using System.Collections;

namespace FleetManagementSystem_.Net.Models
{
    public class StorageSite
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public int MaxVehicleCapacity { get; set; }
        public bool HasRepairStation { get; set; }
        public DateTime? InUseDate { get; set; }
        public DateTime? OutUseDate { get; set; }

        public StorageSite()
        {
            
        }

        public StorageSite(SqlDataReader dataReader, bool isList = false)
        {
            GetColumns(dataReader, isList);
        }

        public void GetColumns(SqlDataReader dataReader, bool isList=false)
        {
            if (!isList)
            {
                dataReader.Read();
            }
            Id = dataReader.Get<Guid>("Id");
            Name = dataReader.Get<string>("Name");
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
