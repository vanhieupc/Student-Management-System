using Microsoft.AspNetCore.Mvc;

namespace BTTH_05.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
