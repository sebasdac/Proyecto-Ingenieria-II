using Microsoft.AspNetCore.Mvc;

namespace ProyectoIngenieria.Controllers
{
    public class InventoriesController : Controller
    {
        public IActionResult Index()
        {
            // Retorna la vista Inventory_management.cshtml
            return View("Inventory_management");
        }
    }
}
