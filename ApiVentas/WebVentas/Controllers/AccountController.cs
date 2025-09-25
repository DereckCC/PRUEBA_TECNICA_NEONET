using Microsoft.AspNetCore.Mvc;

namespace WebVentas.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (email == "admin@mail.com" && password == "admin")
            {
                HttpContext.Session.SetString("User", email);
                return RedirectToAction("Index", "Productos");
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
