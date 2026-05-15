using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class RegisterController : Controller
    {
        private readonly UserManager<FMSUser> _userManager;
        private readonly ILogger<RegisterController> _logger;

        public RegisterController(UserManager<FMSUser> userManager, ILogger<RegisterController> logger)
        {
            _userManager = userManager;
            _logger = logger;
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
                return RedirectToAction("Index", "Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
    
}
