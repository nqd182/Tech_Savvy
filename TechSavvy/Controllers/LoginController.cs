using Microsoft.AspNetCore.Mvc;

namespace TechSavvy.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
