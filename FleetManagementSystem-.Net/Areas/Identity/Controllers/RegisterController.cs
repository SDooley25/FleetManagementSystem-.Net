using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using FleetManagementSystem_.Net.Services;
using FleetManagementSystem_.Net.Models.Enums;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    // Disabled: require a role that will not exist so the register pages are inaccessible while code remains
    [Authorize(Roles = "__Disabled__")]
    internal class RegisterController : Controller
    {
        private readonly UserManager<FMSUser> _userManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly IAlertService _alertService;

        public RegisterController(UserManager<FMSUser> userManager, ILogger<RegisterController> logger, IAlertService alertService)
        {
            _userManager = userManager;
            _logger = logger;
            _alertService = alertService;
        }

        // GET: /Register
        public IActionResult Index()
        {
            return View(new RegisterViewModel());
        }

        // POST: /Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new FMSUser
            {
                UserName = model.Username,
                NormalizedUserName = model.Username?.ToUpperInvariant(),
                Email = model.Email,
                NormalizedEmail = model.Email?.ToUpperInvariant(),
                EmailConfirmed = true, //no access to email service so can't confirm therefore set to true by default
            };

            _logger.LogInformation("Register attempt for {Username}", model.Username);

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Username} created successfully.", model.Username);
                _alertService.AddAlert("Registration successful. You may now log in.", AlertLevel.Success);
                return RedirectToAction("Index", "Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _alertService.AddAlert("Registration failed. See errors.", AlertLevel.Error);

            return View(model);
        }
    }
    
}
