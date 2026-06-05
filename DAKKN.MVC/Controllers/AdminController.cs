using Microsoft.AspNetCore.Mvc;

namespace DAKKN.MVC.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "نظرة عامة";
        return View();
    }

    public IActionResult Users()
    {
        ViewData["Title"] = "المستخدمون";
        return View("Placeholder", "المستخدمون");
    }

    public IActionResult Orders()
    {
        ViewData["Title"] = "الطلبات";
        return View("Placeholder", "الطلبات");
    }

    public IActionResult OrderDetails(int id)
    {
        ViewData["Title"] = "تفاصيل الطلب";
        return View("Placeholder", $"تفاصيل الطلب #{id}");
    }

    public IActionResult Inventory()
    {
        ViewData["Title"] = "المخزون";
        return View("Placeholder", "المخزون");
    }

    public IActionResult Support()
    {
        ViewData["Title"] = "الدعم";
        return View("Placeholder", "الدعم");
    }

    public IActionResult Settings()
    {
        ViewData["Title"] = "الإعدادات";
        return View("Placeholder", "الإعدادات");
    }

    public IActionResult AddProduct()
    {
        ViewData["Title"] = "إضافة منتج";
        return View("Placeholder", "إضافة منتج جديد");
    }
}
