using Microsoft.AspNetCore.Mvc;

namespace ProyectoIngenieria.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Index()
        {
            // Retorna la vista Inventory_management.cshtml
            return View("Invoice_management");
        }
    }
}