using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Areas.Identity.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using FleetManagementSystem_.Net.Models.Enums;
using FleetManagementSystem_.Net.Services;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles = "SuperAdmin")]
    public class CompromisedPasswordController : Controller
    {
        private readonly ICompromisedPasswordRepository _repo;
        private readonly ILogger<CompromisedPasswordController> _logger;
        private readonly IAlertService _alertService;

        public CompromisedPasswordController(ICompromisedPasswordRepository repo, ILogger<CompromisedPasswordController> logger, IAlertService alertService)
        {
            _repo = repo;
            _logger = logger;
            _alertService = alertService;
        }

        // GET: /Identity/CompromisedPassword
        public async Task<IActionResult> Index()
        {
            var items = await _repo.GetAllAsync();
            ViewBag.Count = items?.Count ?? 0;
            return View(items);
        }

        // GET: /Identity/CompromisedPassword/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Identity/CompromisedPassword/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? Password)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("Password", "Password is required");
                return View();
            }

            try
            {
                var hash = ComputeSha1Hex(Password).ToUpperInvariant();
                var item = new CompromisedPassword
                {
                    Id = Guid.NewGuid(),
                    PasswordHash = hash,
                    DateAdded = DateTimeOffset.UtcNow
                };

                await _repo.CreateAsync(item);
                _alertService.AddAlert("Compromised password added.", AlertLevel.Success);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding compromised password");
                ModelState.AddModelError(string.Empty, "Failed to add compromised password.");
                return View();
            }
        }

        // POST: /Identity/CompromisedPassword/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            try
            {
                var rows = await _repo.DeleteAsync(id);
                if (rows > 0)
                {
                    _alertService.AddAlert("Compromised password removed.", AlertLevel.Success);
                }
                else
                {
                    _alertService.AddAlert("No record removed.", AlertLevel.Error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting compromised password {Id}", id);
                _alertService.AddAlert("Error deleting record.", AlertLevel.Error);
            }

            return RedirectToAction(nameof(Index));
        }

        private static string ComputeSha1Hex(string input)
        {
            using var sha1 = SHA1.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha1.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
