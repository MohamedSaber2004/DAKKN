using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.MVC.Controllers
{
    [Route("admin")]
    // [Authorize(Roles = "Admin")]   // wire up once real auth is implemented
    public class AdminController : Controller
    {
        [HttpGet("")]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "نظرة عامة";
            return View();
        }

        [HttpGet("users")]
        public IActionResult Users()
        {
            ViewData["Title"] = "المستخدمون";
            return View("Placeholder", "المستخدمون");
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = "الطلبات";
            return View("Placeholder", "الطلبات");
        }

        [HttpGet("order-details/{id}")]
        public IActionResult OrderDetails(int id)
        {
            ViewData["Title"] = "تفاصيل الطلب";
            return View("Placeholder", $"تفاصيل الطلب #{id}");
        }

        [HttpGet("inventory")]
        public IActionResult Inventory()
        {
            ViewData["Title"] = "المخزون";
            return View("Placeholder", "المخزون");
        }

        [HttpGet("support")]
        public IActionResult Support()
        {
            ViewData["Title"] = "الدعم";
            return View("Placeholder", "الدعم");
        }

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            ViewData["Title"] = "الإعدادات";
            return View();
        }

        [HttpGet("add-product")]
        public IActionResult AddProduct()
        {
            ViewData["Title"] = "إضافة منتج";
            return View("Placeholder", "إضافة منتج جديد");
        }
    }
}
