using DAKKN.Application.Interfaces;
using DAKKN.MVC.ViewModels.Customer;
using DAKKN.MVC.ViewModels.Admin;
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

        [HttpGet("order-details/{id}")]
        public IActionResult OrderDetails(string id)
        {
            ViewData["Title"] = "Order Details #" + id;

            var viewModel = new CustomerOrderDetailsViewModel
            {
                OrderId = id,
                Status = OrderStatus.Processing,
                OrderDate = DateTime.Now.AddDays(-1),
                ShippingAddress = "123 Nile Street, Apartment 4B",
                ShippingGovernorate = "Cairo",
                ShippingPhone = "+20 100 123 4567",
                Subtotal = 220.00m,
                ShippingFee = 25.00m,
                Items = new List<OrderItemViewModel>
                {
                    new OrderItemViewModel {
                        ProductName = "Retro Cassette Sticker",
                        Sku = "STK-RETR-01",
                        Dimensions = "5x5 cm",
                        Quantity = 2,
                        UnitPrice = 70.00m,
                        ImageUrl = "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M"
                    },
                    new OrderItemViewModel {
                        ProductName = "Pixel Heart Sticker",
                        Sku = "STK-PIXL-03",
                        Dimensions = "4x4 cm",
                        Quantity = 1,
                        UnitPrice = 80.00m,
                        ImageUrl = "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M"
                    }
                },
                Logs = new List<OrderLogViewModel>
                {
                    new OrderLogViewModel { Message = "Order received and payment confirmed.", Timestamp = "1 day ago", Actor = "System", IsSystem = true },
                    new OrderLogViewModel { Message = "Preparing your stickers for production.", Timestamp = "20 hours ago", Actor = "System", IsSystem = true }
                }
            };

            return View(viewModel);
        }

        [HttpGet("custom-order")]
        public IActionResult CustomOrder()
        {
            ViewData["Title"] = "Custom Sticker Order";
            return View();
        }

        [HttpGet("cart")]
        public IActionResult Cart()
        {
            ViewData["Title"] = "Shopping Cart";
            return View();
        }

        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            ViewData["Title"] = "Secure Checkout";
            return View();
        }

        [HttpGet("confirmation")]
        public IActionResult OrderConfirmation(string orderId = "DK-9021")
        {
            ViewData["Title"] = "Order Received";
            ViewData["OrderId"] = orderId;
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
