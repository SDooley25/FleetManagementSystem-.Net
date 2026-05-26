using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using FleetManagementSystem_.Net.Areas.Identity.Models;
using FleetManagementSystem_.Net.Services;
using FleetManagementSystem_.Net.Models.Enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using FleetManagementSystem_.Net.Areas.Identity.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace FleetManagementSystem_.Net.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize(Roles ="SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<FMSUser> _userManager;
        private readonly RoleManager<FMSRole> _roleManager;
        private readonly ILogger<UserController> _logger;
        private readonly IAlertService _alertService;
        private const string SessionKey = "FMSUser_Edit_Id";

        public UserController(UserManager<FMSUser> userManager, RoleManager<FMSRole> roleManager, ILogger<UserController> logger, IAlertService alertService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _alertService = alertService;
        }

        // GET: /Identity/User
        public IActionResult Index()
        {
            _logger.LogDebug("Loading user list via UserManager.");

            // Materialize users from UserManager source. Use a lightweight view model for the view.
            var users = _userManager.Users.ToList();

            var model = FMSUser.ListItem.GetList(users);

            return View(model);
        }

        // GET: /Identity/User/Edit/{id?}
        public async Task<IActionResult> Edit(string? id)
        {
            FMSUser user;
            if (string.IsNullOrEmpty(id))
            {
                user = new FMSUser { Id = Guid.NewGuid() };
            }
            else
            {
                user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
            }

            // Prepare view model
            var currentRoles = user.Id != Guid.Empty ? await _userManager.GetRolesAsync(user) : new List<string>();
            var roles = _roleManager.Roles.ToList();

            var vm = new UserEditViewModel(user, roles, currentRoles);            

            // store id in session for verification on submit
            HttpContext.Session.SetString(SessionKey, vm.Id ?? string.Empty);

            return View(vm);
        }

        // POST: /Identity/User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            // Verify session-stored Id matches submitted Id to defend against tampering
            var sessionId = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionId) || sessionId != model.Id)
            {
                ModelState.AddModelError(string.Empty, "User identifier mismatch or session expired.");
            }
            var changedPassword = false;
            if (!string.IsNullOrWhiteSpace(model.Password + model.ConfirmPassword))
            {
                changedPassword = true;
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords Must Match");
                }
            }

            if (!ModelState.IsValid)
            {
                // reload roles list for redisplay
                var rolesAll = _roleManager.Roles.ToList();
                model.Roles = rolesAll.Select(r => new UserEditViewModel.RoleCheckbox
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSelected = model.SelectedRoles != null && model.SelectedRoles.Contains(r.Name)
                }).ToList();

                return View(model);
            }

            var userIdGuid = Guid.TryParse(model.Id, out var parsedId) ? parsedId : Guid.Empty;
            var existing = await _userManager.FindByIdAsync(model.Id);

            if (existing == null)
            {
                // create new user
                var newUser = new FMSUser
                {
                    Id = userIdGuid != Guid.Empty ? userIdGuid : Guid.NewGuid(),
                    UserName = model.UserName,
                    NormalizedUserName = model.UserName?.ToUpperInvariant(),
                    LockoutEnabled = model.LockoutEnabled,
                    AccessFailedCount = model.AccessFailedCount,
                    LockoutEnd = ParseLockoutEnd(model.LockoutEndLocal),
                    EmailConfirmed = true //no access to email server so auto confirm
                };
                
                var createResult = await _userManager.CreateAsync(newUser);
                if (!createResult.Succeeded)
                {
                    foreach (var err in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                    _alertService.AddAlert("Failed to create user.", AlertLevel.Error);
                    // reload roles for redisplay
                    var rolesAll = _roleManager.Roles.ToList();
                    model.Roles = rolesAll.Select(r => new UserEditViewModel.RoleCheckbox
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        IsSelected = model.SelectedRoles != null && model.SelectedRoles.Contains(r.Name)
                    }).ToList();
                    return View(model);
                }

                if (changedPassword)
                {
                    await _userManager.AddPasswordAsync(newUser, model.Password);
                }

                // handle roles
                if (model.SelectedRoles != null && model.SelectedRoles.Length > 0)
                {
                    foreach (var roleName in model.SelectedRoles)
                    {
                        if (!string.IsNullOrEmpty(roleName))
                        {
                            await _userManager.AddToRoleAsync(newUser, roleName);
                        }
                    }
                }
            }
            else
            {
                // update existing
                existing.UserName = model.UserName;
                existing.NormalizedUserName = model.UserName?.ToUpperInvariant();
                existing.LockoutEnabled = model.LockoutEnabled;
                existing.AccessFailedCount = model.AccessFailedCount;
                existing.LockoutEnd = ParseLockoutEnd(model.LockoutEndLocal);
                existing.EmailConfirmed = true; //no access to email server so auto confirm

                var updateResult = await _userManager.UpdateAsync(existing);
                if (!updateResult.Succeeded)
                {
                    foreach (var err in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, err.Description);
                    }
                    _alertService.AddAlert("Failed to update user.", AlertLevel.Error);
                    // reload roles for redisplay
                    var rolesAll = _roleManager.Roles.ToList();
                    model.Roles = rolesAll.Select(r => new UserEditViewModel.RoleCheckbox
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Description = r.Description,
                        IsSelected = model.SelectedRoles != null && model.SelectedRoles.Contains(r.Name)
                    }).ToList();
                    return View(model);
                }

                if (changedPassword)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(existing);
                    await _userManager.ResetPasswordAsync(existing, token,model.Password);
                }

                // update roles membership
                var currentRoles = await _userManager.GetRolesAsync(existing);
                var selected = model.SelectedRoles?.ToList() ?? new List<string>();
                var toAdd = selected.Except(currentRoles).ToList();
                var toRemove = currentRoles.Except(selected).ToList();

                foreach (var r in toAdd)
                {
                    if (!string.IsNullOrEmpty(r))
                        await _userManager.AddToRoleAsync(existing, r);
                }
                foreach (var r in toRemove)
                {
                    if (!string.IsNullOrEmpty(r))
                        await _userManager.RemoveFromRoleAsync(existing, r);
                }
            }

            // done - remove session key and redirect
            HttpContext.Session.Remove(SessionKey);
            _alertService.AddAlert("User saved successfully.", AlertLevel.Success);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Identity/User/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var existing = await _userManager.FindByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(existing);
            if (!result.Succeeded)
            {
                _alertService.AddAlert("Failed to delete user.", AlertLevel.Error);
                return RedirectToAction(nameof(Index));
            }

            _alertService.AddAlert("User deleted successfully.", AlertLevel.Success);
            return RedirectToAction(nameof(Index));
        }

        private static DateTimeOffset? ParseLockoutEnd(string? lockoutLocalString)
        {
            if (string.IsNullOrEmpty(lockoutLocalString))
                return null;

            // parse as local datetime (format yyyy-MM-ddTHH:mm)
            if (DateTime.TryParse(lockoutLocalString, out var dt))
            {
                var dto = new DateTimeOffset(dt.ToLocalTime());
                return dto;
            }
            return null;
        }

        
    }
}
