using FleetManagementSystem_.Net.Extensions;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Areas.Identity.Models
{
    public sealed class FMSUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? UserName { get; set; }
        public string? NormalizedUserName { get; set; }
        [Required]
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

        public FMSUser(SqlDataReader dataReader, bool isList=false)
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

        public static List<FMSUser> GetListColumns(SqlDataReader dataReader)
        {
            List<FMSUser> list = new List<FMSUser>();
            while (dataReader.Read())
            {
                FMSUser item = new FMSUser(dataReader,true);
                list.Add(item);
            }
            return list;
        }

        // Lightweight view model used by the view
    public class ListItem
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }

        public void Fill(FMSUser item) 
        { 
            Id=item.Id.ToString();
            UserName=item.UserName;
            LockoutEnd=item.LockoutEnd;
            AccessFailedCount=item.AccessFailedCount;
        }

        public static List<ListItem> GetList(List<FMSUser> items)
        {
            List<ListItem> list = new List<ListItem>();
            foreach (var item in items)
            {
                ListItem viewItem = new ListItem();
                viewItem.Fill(item);
                list.Add(viewItem);
            }
            return list;
        }
    }
    }

    
}
