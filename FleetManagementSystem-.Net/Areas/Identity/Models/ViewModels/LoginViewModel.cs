using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        // Optional return URL to redirect after successful login
        public string? ReturnUrl { get; set; }
    }
}
