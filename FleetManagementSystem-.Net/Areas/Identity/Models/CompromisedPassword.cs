using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Areas.Identity.Models
{
    public sealed class CompromisedPassword
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTimeOffset DateAdded { get; set; } = DateTimeOffset.UtcNow;

        public CompromisedPassword()
        {
        }

        public CompromisedPassword(SqlDataReader dataReader, bool isList = false)
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
            PasswordHash = dataReader.Get<string>("PasswordHash");
            DateAdded = dataReader.Get<DateTime>("DateAdded");
        }

        public static List<CompromisedPassword> GetListColumns(SqlDataReader dataReader)
        {
            List<CompromisedPassword> list = new List<CompromisedPassword>();
            while (dataReader.Read())
            {
                CompromisedPassword item = new CompromisedPassword(dataReader, true);
                list.Add(item);
            }
            return list;
        }
    }
}
