using DAKKN.Application.Localization;
using DAKKN.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Diagnostics;

namespace DAKKN.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [OutputCache(Duration = 600)]
        public IActionResult Index()
        {
            // Redirect authorized users to the Customer Dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Customer");
            }

            return View();
        }

        [OutputCache(Duration = 600)]
        public IActionResult Privacy()
        {
            return View();
        }

        [OutputCache(Duration = 600)]
        public IActionResult Terms()
        {
            return View();
        }

        [OutputCache(Duration = 600)]
        public IActionResult About()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
