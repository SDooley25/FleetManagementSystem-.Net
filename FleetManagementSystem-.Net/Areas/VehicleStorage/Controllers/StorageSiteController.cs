using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using FleetManagementSystem_.Net.Models.Enums;

namespace FleetManagementSystem_.Net.Areas.VehicleStorage.Controllers
{
    [Area("VehicleStorage")]
    [Authorize(Roles = "VehicleStorage.Edit")]
    public class StorageSiteController : Controller
    {
        private readonly IStorageSiteRepository _repository;
        private readonly ILogger<StorageSiteController> _logger;
        private readonly IAlertService _alertService;
        private const string SessionKey = "StorageSite.Id";

        public StorageSiteController(IStorageSiteRepository repository, ILogger<StorageSiteController> logger, IAlertService alertService)
        {
            _repository = repository;
            _logger = logger;
            _alertService = alertService;
        }

        // Index = list
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync(cancellationToken);
            return View(list);
        }

        // GET: Edit (create if id is null)
        public async Task<IActionResult> Edit(Guid? id, CancellationToken cancellationToken)
        {
            StorageSite site;
            if (id == null || id == Guid.Empty)
            {
                // create new
                site = new StorageSite
                {
                    Id = Guid.NewGuid()
                };
            }
            else
            {
                site = await _repository.GetByIdAsync(id.Value, cancellationToken) ?? new StorageSite { Id = id.Value };
            }

            // store id in session for verification on POST
            HttpContext.Session.SetString(SessionKey, site.Id.ToString());

            return View(site);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StorageSite model, CancellationToken cancellationToken)
        {            
            cancellationToken.ThrowIfCancellationRequested();

            var sessionId = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionId) || !Guid.TryParse(sessionId, out var sessionGuid))
            {
                ModelState.AddModelError(string.Empty, "Session validation failed (missing ID).");
            }
            else if (sessionGuid != model.Id)
            {
                ModelState.AddModelError(string.Empty, "Session validation failed (ID mismatch).");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // decide create or update by checking existence (simple heuristic: attempt update; if fails insert)
            // Better approach could be to use a dedicated create flow, but following user's requirement to use edit for create.
            var existing = await _repository.GetByIdAsync(model.Id, cancellationToken);
            if (existing == null)
            {
                var newId = await _repository.InsertAsync(model, cancellationToken);
                if (newId == Guid.Empty)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create storage site.");
                    return View(model);
                }
                _alertService.AddAlert("Storage site created successfully.",AlertLevel.Success);
            }
            else
            {
                var success = await _repository.UpdateAsync(model, cancellationToken);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update storage site.");
                    return View(model);
                }
                _alertService.AddAlert("Storage site updated successfully.", AlertLevel.Success);
            }

            // clear session id as it's no longer needed
            HttpContext.Session.Remove(SessionKey);

            return RedirectToAction(nameof(Index));
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "VehicleStorage.Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var site = await _repository.GetByIdAsync(id, cancellationToken);
            if (site == null)
            {
                return NotFound();
            }

            var deleted = await _repository.DeleteAsync(site, cancellationToken);
            if (!deleted)
            {
                _alertService.AddAlert("Could not delete storage site.", AlertLevel.Error);
            }
            else
            {
                _alertService.AddAlert("Storage site deleted successfully.", AlertLevel.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}