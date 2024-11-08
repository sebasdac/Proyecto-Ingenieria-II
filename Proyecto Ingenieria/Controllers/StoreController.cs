using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
