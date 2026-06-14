using DAKKN.Application.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.MVC.ViewModels.Customer;
using DAKKN.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;

namespace DAKKN.MVC.Controllers
{
    [RoleAuthorize(UserType.User, UserType.Admin)]
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

        [HttpGet("favorites")]
        public IActionResult Favorites()
        {
            ViewData["Title"] = "My Favorites";
            var viewModel = new FavoritesViewModel
            {
                FavoriteProducts = new List<ProductDto>
                {
                    new ProductDto
                    {
                        Id = Guid.Parse("f4d6c75f-2643-4ffc-8199-e94a91d8b617"),
                        Name = "Retro Cassette",
                        Price = 70,
                        ImageUrls = new List<string> { "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M" },
                        FinishOptions = new List<string> { "Holographic" }
                    },
                    new ProductDto
                    {
                        Id = Guid.Parse("67890abc-def0-1234-5678-90abcdef1234"),
                        Name = "Pixel Heart",
                        Price = 60,
                        ImageUrls = new List<string> { "https://lh3.googleusercontent.com/aida/AP1WRLvk0o2fHlsgnzmZ9xrrRAnUTq9hl5CW063LX44OhYiXj0ZAadSE46fIAmtHQKBEzfk8NKsMl6AwohhVGiDgxsbst33TGFkDW68Y9_d3C1Cvf-mtviXqQYHM-Aqnsunu3Q6Yy2-9A5mXcsO80D173UvbE44Odz0GI41kQbHK_cLtDiruHCKTT15snGH5XT7Y6MLMfFd4rUCTY71ZJwiOba19bKW0Du2pMXYtJtF-I-h5V-wypv1z0Vztp_M" },
                        FinishOptions = new List<string> { "Clear Vinyl" }
                    }
                }
            };
            return View(viewModel);
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

        [HttpGet("support")]
        public IActionResult Support()
        {
            ViewData["Title"] = "Support Center";
            var viewModel = new CustomerSupportDashboardViewModel
            {
                Tickets = new List<SupportTicketViewModel>
                {
                    new SupportTicketViewModel { TicketId = "TK-2024-001", Subject = "Missing Item in Order #DK-9021", Status = TicketStatus.Open, Priority = TicketPriority.High, CreatedAt = DateTime.Now.AddDays(-1) },
                    new SupportTicketViewModel { TicketId = "TK-2024-002", Subject = "Holographic finish question", Status = TicketStatus.Resolved, Priority = TicketPriority.Low, CreatedAt = DateTime.Now.AddDays(-5) }
                }
            };
            return View(viewModel);
        }

        [HttpGet("support/new")]
        public IActionResult NewTicket(string? orderId)
        {
            ViewData["Title"] = "Open New Ticket";
            return View(new NewTicketViewModel { OrderId = orderId });
        }

        [HttpPost("support/new")]
        public IActionResult CreateTicket(NewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Open New Ticket";
                return View("NewTicket", model);
            }

            // Mock saving ticket
            TempData["SuccessMessage"] = "Your support ticket has been created successfully!";
            return RedirectToAction("Support");
        }
    }
}
