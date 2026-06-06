using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.MVC.Controllers
{
    [Authorize]
    [Route("customer")]
    public class CustomerController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Customer Dashboard";
            return View();
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            ViewData["Title"] = "Browse Stickers";
            return View();
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = "My Orders";
            return View();
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            ViewData["Title"] = "My Profile";
            return View();
        }
    }
}
