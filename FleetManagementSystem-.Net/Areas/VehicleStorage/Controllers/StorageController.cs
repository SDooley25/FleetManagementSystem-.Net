using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FleetManagementSystem_.Net.Models.Enums;
using FleetManagementSystem_.Net.Areas.VehicleStorage.ViewModels;

namespace FleetManagementSystem_.Net.Areas.VehicleStorage.Controllers
{
    [Area("VehicleStorage")]
    [Authorize(Roles = "VehicleStorage.Edit")]
    public class StorageController : Controller
    {
        private readonly IVehicleStorageRepository _repository;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IStorageSiteRepository _siteRepo;
        private readonly ILogger<StorageController> _logger;
        private readonly IAlertService _alertService;
        private const string SessionKey = "VehicleStorage.Id";

        public StorageController(IVehicleStorageRepository repository, IVehicleRepository vehicleRepo, IStorageSiteRepository siteRepo, ILogger<StorageController> logger, IAlertService alertService)
        {
            _repository = repository;
            _vehicleRepo = vehicleRepo;
            _siteRepo = siteRepo;
            _logger = logger;
            _alertService = alertService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync(cancellationToken);
            return View(list);
        }

        public async Task<IActionResult> Edit(Guid? id, CancellationToken cancellationToken)
        {
            VehicleStorageEditViewModel vm;
            if (id == null || id == Guid.Empty)
            {
                vm = new VehicleStorageEditViewModel { Id = Guid.NewGuid(), StartDate = DateTime.Today };
            }
            else
            {
                var item = await _repository.GetByIdAsync(id.Value, cancellationToken) ?? new Models.VehicleStorage { Id = id.Value };
                vm = new VehicleStorageEditViewModel
                {
                    Id = item.Id,
                    VehicleId = item.Vehicle?.Id ?? Guid.Empty,
                    StorageSiteId = item.StorageSite?.Id ?? Guid.Empty,
                    StorageType = item.StorageType,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    Note = item.Note
                };
            }

            HttpContext.Session.SetString(SessionKey, vm.Id.ToString());

            vm.Vehicles = await _vehicleRepo.GetAllAsync(cancellationToken);
            vm.Sites = await _siteRepo.GetAllAsync(cancellationToken);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleStorageEditViewModel model, CancellationToken cancellationToken)
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
                model.Vehicles = await _vehicleRepo.GetAllAsync(cancellationToken);
                model.Sites = await _siteRepo.GetAllAsync(cancellationToken);
                return View(model);
            }
            // map viewmodel to domain model
            var domain = new Models.VehicleStorage
            {
                Id = model.Id,
                Vehicle = new Vehicle { Id = model.VehicleId },
                StorageSite = new StorageSite { Id = model.StorageSiteId },
                StorageType = model.StorageType,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Note = model.Note
            };

            var existing = await _repository.GetByIdAsync(model.Id, cancellationToken);
            if (existing == null)
            {
                var newId = await _repository.InsertAsync(domain, cancellationToken);
                if (newId == Guid.Empty)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create vehicle storage.");
                    model.Vehicles = await _vehicleRepo.GetAllAsync(cancellationToken);
                    model.Sites = await _siteRepo.GetAllAsync(cancellationToken);
                    return View(model);
                }
                _alertService.AddAlert("Vehicle storage created successfully.", FleetManagementSystem_.Net.Models.Enums.AlertLevel.Success);
            }
            else
            {
                var success = await _repository.UpdateAsync(domain, cancellationToken);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update vehicle storage.");
                    model.Vehicles = await _vehicleRepo.GetAllAsync(cancellationToken);
                    model.Sites = await _siteRepo.GetAllAsync(cancellationToken);
                    return View(model);
                }
                _alertService.AddAlert("Vehicle storage updated successfully.", AlertLevel.Success);
            }

            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "VehicleStorage.Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var item = await _repository.GetByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound();
            }

            var deleted = await _repository.DeleteAsync(item, cancellationToken);
            if (!deleted)
            {
                _alertService.AddAlert("Could not delete vehicle storage.", AlertLevel.Error);
            }
            else
            {
                _alertService.AddAlert("Vehicle storage deleted successfully.", AlertLevel.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
