using DAKKN.Application.Interfaces;
using DAKKN.MVC.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.MVC.Controllers
{
    [Authorize]
    [Route("customer")]
    public class CustomerController(IProductService productService, IDashboardService dashboardService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Customer Dashboard";
            var dashboardData = await dashboardService.GetCustomerDashboardDataAsync(Guid.Empty); // Using empty Guid for mock
            return View(new CustomerDashboardViewModel { Dashboard = dashboardData });
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            ViewData["Title"] = "Browse Stickers";
            return View();
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            ViewData["Title"] = product.Name;
            return View(new ProductDetailsViewModel { Product = product });
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
