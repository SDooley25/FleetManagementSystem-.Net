using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FleetManagementSystem_.Net.Areas.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles ="SuperAdmin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<FMSRole> _roleManager;

        public RoleController(RoleManager<FMSRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: /Identity/Role
        public IActionResult Index()
        {
            // RoleManager.Roles returns IQueryable<FMSRole>
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        // GET: /Identity/Role/Edit/{id?}
        // If id is null or empty - create new role (Id generated)
        public async Task<IActionResult> Edit(string? id)
        {
            FMSRole role;
            if (string.IsNullOrEmpty(id))
            {
                // Create new role with fresh Id
                role = new FMSRole { Id = Guid.NewGuid() };
            }
            else
            {
                role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
            }

            // Store the expected Id in session for verification at POST
            HttpContext.Session.SetString("FMSRole_Edit_Id", role.Id.ToString());

            return View(role);
        }

        // POST: /Identity/Role/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FMSRole model)
        {
            // Verify session-stored Id matches submitted Id to defend against tampering
            var sessionId = HttpContext.Session.GetString("FMSRole_Edit_Id");
            if (string.IsNullOrEmpty(sessionId) || !Guid.TryParse(sessionId, out var sessionGuid) || sessionGuid != model.Id)
            {
                ModelState.AddModelError(string.Empty, "Role identifier mismatch or session expired.");
                return View(model);
            }

            model.NormalizedName= model.Name?.ToUpperInvariant();

            // Determine create vs update by checking existence
            var existing = await _roleManager.FindByIdAsync(model.Id.ToString());
            if (existing == null)
            {
                // Create new
                var createResult = await _roleManager.CreateAsync(model);
                if (createResult.Succeeded)
                {
                    HttpContext.Session.Remove("FMSRole_Edit_Id");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var err in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }
            else
            {
                // Update existing
                existing.Name = model.Name;
                existing.NormalizedName = model.NormalizedName;
                existing.Description = model.Description;

                var updateResult = await _roleManager.UpdateAsync(existing);
                if (updateResult.Succeeded)
                {
                    HttpContext.Session.Remove("FMSRole_Edit_Id");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var err in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
                return View(model);
            }
        }
    }
}
