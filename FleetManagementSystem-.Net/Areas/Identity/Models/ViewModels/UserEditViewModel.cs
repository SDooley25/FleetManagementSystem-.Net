using System.Data;

namespace FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels
{
    // View model for the Edit page
    public class UserEditViewModel
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool LockoutEnabled { get; set; }
        public string? LockoutEndLocal { get; set; } // binds to datetime-local input
        public int AccessFailedCount { get; set; }
        public List<RoleCheckbox> Roles { get; set; } = new();
        public string[]? SelectedRoles { get; set; } 
        public class RoleCheckbox
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public bool IsSelected { get; set; }
        }

        public UserEditViewModel()
        {
            
        }

        public UserEditViewModel(FMSUser user, List<FMSRole> allRoles, IList<string> selectedRoles)
        {
            Id = user.Id.ToString();
            UserName = user.UserName;
            LockoutEnabled = user.LockoutEnabled;
            // Format for datetime-local input (local time)
            LockoutEndLocal = user.LockoutEnd?.ToLocalTime().ToString("yyyy-MM-dd'T'HH:mm");
            AccessFailedCount = user.AccessFailedCount;
            Roles = allRoles.Select(r => new UserEditViewModel.RoleCheckbox
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                IsSelected = !string.IsNullOrEmpty(r.Name) && selectedRoles.Contains(r.Name)
            }).ToList();
        }
    }        
}
