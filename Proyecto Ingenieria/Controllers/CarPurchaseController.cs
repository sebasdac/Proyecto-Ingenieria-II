using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Ingenieria.Controllers
{
    public class CarPurchaseController : Controller
    {
        public IActionResult Index(int? id)
        {
            if (id.HasValue)
            {
                ViewBag.CarId = id.Value; // Pasar el ID a la vista
            }
            
            return View();
        }
    }
}
