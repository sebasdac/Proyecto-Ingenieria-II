using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Ingenieria.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
