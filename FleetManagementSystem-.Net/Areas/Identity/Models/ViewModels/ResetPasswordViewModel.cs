using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public string? ConfirmNewPassword { get; set; }

        public bool IsSamePassword(string? currentPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(currentPasswordHash) || string.IsNullOrWhiteSpace(NewPassword))
            {
                return false;
            }

            if (!currentPasswordHash.StartsWith("AQAAAA", StringComparison.Ordinal))
            {
                return false;
            }

            try
            {
                return PasswordHasherCompare(currentPasswordHash, NewPassword);
            }
            catch
            {
                return false;
            }
        }

        private static bool PasswordHasherCompare(string hashedPassword, string providedPassword)
        {
            var hasher = new PasswordHasher<FMSUser>();
            return hasher.VerifyHashedPassword(new FMSUser(), hashedPassword, providedPassword) == PasswordVerificationResult.Success;
        }
    }
}