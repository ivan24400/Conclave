using Microsoft.AspNetCore.Mvc;

namespace Conclave.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult About()
        {
            return new JsonResult(new { name = "ConclaveAPI", version = "1.0" });
        }
    }
}