using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Models.Enums;
using FleetManagementSystem_.Net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FleetManagementSystem_.Net.Areas.VehicleStorage.Controllers
{
    [Area("VehicleStorage")]
    [Authorize(Roles = "VehicleStorage.Edit")]
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _repository;
        private readonly IVehicleStorageRepository _vehicleStorageRepository;
        private readonly ILogger<VehicleController> _logger;
        private readonly IAlertService _alertService;
        private const string SessionKey = "Vehicle.Id";

        public VehicleController(IVehicleRepository repository, IVehicleStorageRepository vehicleStorageRepository, ILogger<VehicleController> logger, IAlertService alertService)
        {
            _repository = repository;
            _vehicleStorageRepository = vehicleStorageRepository;
            _logger = logger;
            _alertService = alertService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync(cancellationToken);
            return View(list);
        }

        // GET: Edit (create if id null)
        public async Task<IActionResult> Edit(Guid? id, CancellationToken cancellationToken)
        {
            Vehicle v;
            if (id == null || id == Guid.Empty)
            {
                v = new Vehicle { Id = Guid.NewGuid() };
            }
            else
            {
                v = await _repository.GetByIdAsync(id.Value, cancellationToken) ?? new Vehicle { Id = id.Value };
            }

            HttpContext.Session.SetString(SessionKey, v.Id.ToString());

            return View(v);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Vehicle vehicle, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sessionId = HttpContext.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionId) || !Guid.TryParse(sessionId, out var sessionGuid))
            {
                ModelState.AddModelError(string.Empty, "Session validation failed (missing ID).");
            }
            else if (sessionGuid != vehicle.Id)
            {
                ModelState.AddModelError(string.Empty, "Session validation failed (ID mismatch).");
            }

            if (!ModelState.IsValid)
            {
                // Log ModelState errors and submitted form values to help diagnose binding issues
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Where(s => !string.IsNullOrEmpty(s));
                _logger.LogWarning("Vehicle Edit: ModelState invalid. Errors: {Errors}", string.Join("; ", errors));
                foreach (var kv in Request.Form)
                {
                    _logger.LogDebug("Form data: {Key} = {Value}", kv.Key, kv.Value.ToString());
                }
                return View(vehicle);
            }

            var existing = await _repository.GetByIdAsync(vehicle.Id, cancellationToken);
            if (existing == null)
            {
                var newId = await _repository.InsertAsync(vehicle, cancellationToken);
                if (newId == Guid.Empty)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create vehicle.");
                    return View(vehicle);
                }
                _alertService.AddAlert("Vehicle created successfully.", AlertLevel.Success);
            }
            else
            {
                var success = await _repository.UpdateAsync(vehicle, cancellationToken);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update vehicle.");
                    return View(vehicle);
                }
                _alertService.AddAlert("Vehicle updated successfully.", AlertLevel.Success);
            }

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
            var v = await _repository.GetByIdAsync(id, cancellationToken);
            if (v == null)
            {
                return NotFound();
            }

            var storageEntries = await _vehicleStorageRepository.GetByVehicleAsync(v.Id, cancellationToken);
            if (storageEntries.Any())
            {
                _alertService.AddAlert($"Vehicle '{v.RegistrationNumber}' cannot be deleted because it is used in vehicle storage.", AlertLevel.Error);
                return RedirectToAction(nameof(Index));
            }

            var deleted = await _repository.DeleteAsync(v, cancellationToken);
            if (!deleted)
            {
                _alertService.AddAlert("Could not delete vehicle.", AlertLevel.Error);
            }
            else
            {
                _alertService.AddAlert("Vehicle deleted successfully.", AlertLevel.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
