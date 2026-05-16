using FleetManagementSystem_.Net.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Areas.Identity.Models
{
    public sealed class FMSRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        [Required]
        public string? Description { get; set; }

        public FMSRole() { }

        public FMSRole(SqlDataReader dataReader, bool isList=false)
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
            NormalizedName = dataReader.Get<string>("NormalizedName");
            Description = dataReader.Get<string>("Description");
        }

        public static List<FMSRole> GetListColumns(SqlDataReader dataReader)
        {
            var list = new List<FMSRole>();
            while (dataReader.Read())
            {
                var item = new FMSRole(dataReader, true);                   
                list.Add(item);
            }
            return list;
        }
    }
}
