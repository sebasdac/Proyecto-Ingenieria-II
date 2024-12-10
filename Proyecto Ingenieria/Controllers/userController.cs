using Microsoft.AspNetCore.Mvc;

namespace Proyecto.Controllers
{
    public class userController : Controller
    {
        public IActionResult Index()
        {
            // Obtener datos del usuario desde el token
            var userId = User.FindFirst("UserId")?.Value; // ID del usuario
            var username = User.Identity.Name; // Nombre del usuario

            // Pasar datos a la vista
            ViewBag.Username = username;
            ViewBag.UserId = userId;

            return View();
        }
        public IActionResult Facturas()
        {
            return View();
        }
        public IActionResult Forms()
        {
            return View();
        }
        public IActionResult DashBoard()
        {
            return View();
        }
        public IActionResult Carros()
        {
            return View();
        }
        public IActionResult Icons()
        {
            return View();
        }
        public IActionResult Buttons()
        {
            return View();
        }
        public IActionResult Dropdowns()
        {
            return View();
        }
        public IActionResult Typography()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }


    }
}
