using Microsoft.AspNetCore.Mvc;

namespace FleetManagementSystem_.Net.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult SQLTimeout()
        {
            return View();
        }
    }
}
