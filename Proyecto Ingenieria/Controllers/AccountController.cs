using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    // Acción para mostrar la vista de inicio de sesión
    public IActionResult Login()
    {
        return View(); // Esto buscará la vista Login.cshtml en Views/Account/
    }

    public  IActionResult register()
    {
        return View();
    }
}
