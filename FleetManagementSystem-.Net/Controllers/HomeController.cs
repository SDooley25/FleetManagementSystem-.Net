using FleetManagementSystem_.Net.Models;
using FleetManagementSystem_.Net.Data;
using FleetManagementSystem_.Net.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetManagementSystem_.Net.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IStorageSiteRepository _siteRepository;
        private readonly IVehicleStorageRepository _storageRepository;
        private readonly UserManager<FMSUser> _userManager;

        public HomeController(IVehicleRepository vehicleRepository, IStorageSiteRepository siteRepository, IVehicleStorageRepository storageRepository, UserManager<FMSUser> userManager)
        {
            _vehicleRepository = vehicleRepository;
            _siteRepository = siteRepository;
            _storageRepository = storageRepository;
            _userManager= userManager;
        }
        [Authorize]

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new DashboardViewModel();
            try
            { 
                model.UserCount = _userManager.Users.Count(); 
            }
            catch
            {
                model.UserCount = 0;
            }

            try
            {
                var vehicles = await _vehicleRepository.GetAllAsync(cancellationToken);
                model.VehicleCount = vehicles?.Count ?? 0;
            }
            catch
            {
                model.VehicleCount = 0;
            }

            try
            {
                var sites = await _siteRepository.GetAllAsync(cancellationToken);
                model.StorageSiteCount = sites?.Count ?? 0;
            }
            catch
            {
                model.StorageSiteCount = 0;
            }

            try
            {
                var storages = await _storageRepository.GetAllAsync(cancellationToken);
                model.TotalStorages = storages?.Count ?? 0;
                model.TemporaryStorages = storages?.Count(s => s.StorageType == Models.Enums.StorageType.Temporary) ?? 0;
                model.ActiveStorages = storages?.Count(s => !s.EndDate.HasValue || s.EndDate > DateTime.Now) ?? 0;
            }
            catch
            {
                model.TotalStorages = 0;
                model.TemporaryStorages = 0;
                model.ActiveStorages = 0;
            }

            return View(model);
        }
        // Disabled: make Privacy inaccessible by requiring a non-existent role
        [Authorize(Roles ="__Disabled__")]
        private IActionResult Privacy()
        {
            return View();
        }

        // New access denied page
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
