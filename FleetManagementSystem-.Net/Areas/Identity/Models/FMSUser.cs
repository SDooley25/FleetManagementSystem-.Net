using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;

namespace FleetManagementSystem_.Net.Areas.Identity.Models
{
    public sealed class FMSUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }

        public FMSUser()
        {
            
        }

        public FMSUser(SqlDataReader dataReader)
        {
            GetColumns(dataReader);
        }
        public void GetColumns(SqlDataReader dataReader, bool isList=false)
        {
            if (!isList)
            {
                dataReader.Read();
            }
            Id = dataReader.Get<Guid>("Id");
            UserName = dataReader.Get<string>("UserName");
            NormalizedUserName = dataReader.Get<string>("NormalizedUserName");
            Email = dataReader.Get<string>("Email");
            NormalizedEmail = dataReader.Get<string>("NormalizedEmail");
            EmailConfirmed = dataReader.Get<bool>("EmailConfirmed");
            PasswordHash = dataReader.Get<string>("PasswordHash");
            LockoutEnabled = dataReader.Get<bool>("LockoutEnabled");
            LockoutEnd = dataReader.Get<DateTimeOffset?>("LockoutEnd");
            AccessFailedCount = dataReader.Get<int>("AccessFailedCount");
        }
    }
}
