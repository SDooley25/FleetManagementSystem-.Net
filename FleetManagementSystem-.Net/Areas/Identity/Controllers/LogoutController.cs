using FleetManagementSystem_.Net.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class LogoutController : Controller
    {
        private readonly SignInManager<FMSUser> _signInManager;
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(SignInManager<FMSUser> signInManager, ILogger<LogoutController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        //// GET: /Identity/Logout
        //// Shows a confirmation page. If you prefer a direct POST-only logout, remove this action and post directly.
        //[HttpGet]
        //public IActionResult Index(string? returnUrl = null)
        //{
        //    ViewData["ReturnUrl"] = returnUrl;
        //    return View();
        //}

        // POST: /Identity/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexPost(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/");
        }
    }
}
