using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using FleetManagementSystem_.Net.Models.Enums;
using FleetManagementSystem_.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class PasswordController : Controller
    {
        private readonly UserManager<FMSUser> _userManager;
        private readonly SignInManager<FMSUser> _signInManager;
        private readonly IAlertService _alertService;
        private readonly ILogger<PasswordController> _logger;

        public PasswordController(
            UserManager<FMSUser> userManager,
            SignInManager<FMSUser> signInManager,
            IAlertService alertService,
            ILogger<PasswordController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _alertService = alertService;
            _logger = logger;
        }

        // GET: /Identity/Password
        public IActionResult Index()
        {
            return View(new ResetPasswordViewModel());
        }

        // POST: /Identity/Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            if (model.IsSamePassword(user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.NewPassword), "New password must be different from the current password.");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword ?? string.Empty,
                model.NewPassword ?? string.Empty);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                _alertService.AddAlert("Password reset failed.", AlertLevel.Error);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Password reset for user {UserName}", user.UserName);
            _alertService.AddAlert("Password updated successfully.", AlertLevel.Success);
            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }

        
    }
}