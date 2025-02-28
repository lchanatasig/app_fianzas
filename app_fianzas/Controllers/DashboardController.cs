using Microsoft.AspNetCore.Mvc;

namespace app_fianzas.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet("Inicio")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
