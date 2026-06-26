using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using FleetManagementSystem_.Net.Areas.Identity.Services;
using FleetManagementSystem_.Net.Models.Enums;
using FleetManagementSystem_.Net.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<FMSUser> _signInManager;
        private readonly ICompromisedPasswordService _compromisedPasswordService;
        private readonly IAlertService _alertService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            SignInManager<FMSUser> signInManager,
            ICompromisedPasswordService compromisedPasswordService,
            IAlertService alertService,
            ILogger<LoginController> logger)
        {
            _signInManager = signInManager;
            _compromisedPasswordService = compromisedPasswordService;
            _alertService = alertService;
            _logger = logger;
        }

        // GET: /Identity/Login
        public IActionResult Index(string? returnUrl = null)
        {
            var viewModel = new LoginViewModel { ReturnUrl = returnUrl };
            return View(viewModel);
        }

        // POST: /Identity/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Login attempt for {Username}", model.Username);

            var result = await _signInManager.PasswordSignInAsync(
                model.Username ?? string.Empty,
                model.Password ?? string.Empty,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Username} logged in.", model.Username);

                if (await _compromisedPasswordService.IsCompromisedAsync(model.Password ?? string.Empty))
                {
                    _alertService.AddAlert("Your password is on the compromised list. Reset it now.", AlertLevel.Warning);
                    return RedirectToAction("Index", "Password", new { area = "Identity" });
                }

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return Redirect("/");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "User account locked out.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Login not allowed. Confirm your account or contact administrator.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        // POST: /Identity/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
