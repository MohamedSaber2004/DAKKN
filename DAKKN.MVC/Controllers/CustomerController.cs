using DAKKN.Application.DTOs;
using DAKKN.Application.Common.Exceptions;
using DAKKN.MVC.ViewModels.Customer;
using DAKKN.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using Microsoft.Extensions.Localization;
using DAKKN.Application.Localization;
using MediatR;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.Products.Queries.GetProductById;

namespace DAKKN.MVC.Controllers
{
    [RoleAuthorize(UserType.User, UserType.Admin)]
    [Route("customer")]
    public class CustomerController(IStringLocalizer<Messages> localizer, IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = localizer["Dashboard_Welcome"];

            var products = await mediator.Send(new GetProductsQuery(null, null, 1, 10));
            var dashboard = new DashboardDto
            {
                Recommendations = products.Items.Take(4).ToList(),
                ProgrammingStickers = new List<ProductDto>(),
                MemeStickers = new List<ProductDto>()
            };

            return View(new CustomerDashboardViewModel { Dashboard = dashboard });
        }

        [HttpGet("products")]
        public IActionResult Products()
        {
            ViewData["Title"] = localizer["nav_shop"];
            return View();
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            try
            {
                var product = await mediator.Send(new GetProductByIdQuery(id));
                ViewData["Title"] = product.Name;
                return View(new ProductDetailsViewModel { Product = product });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = localizer["admin_orders"];
            return View();
        }

        [HttpGet("favorites")]
        public IActionResult Favorites()
        {
            ViewData["Title"] = localizer["nav_favorites"];
            var viewModel = new FavoritesViewModel
            {
                FavoriteProducts = new List<ProductDto>
                {
                    new ProductDto
                    {
                        Id = Guid.Parse("f4d6c75f-2643-4ffc-8199-e94a91d8b617"),
                        Name = "Retro Cassette",
                        Price = 70,
                        ImageUrl = "https://images.unsplash.com/photo-1605648916319-cf082f7524a1?q=80&w=400&auto=format&fit=crop",
                        FinishOptions = new List<string> { "Holographic" }
                    },
                    new ProductDto
                    {
                        Id = Guid.Parse("67890abc-def0-1234-5678-90abcdef1234"),
                        Name = "Pixel Heart",
                        Price = 60,
                        ImageUrl = "https://images.unsplash.com/photo-1599305090598-fe179d501c27?q=80&w=400&auto=format&fit=crop",
                        FinishOptions = new List<string> { "Clear Vinyl" }
                    }
                }
            };
            return View(viewModel);
        }

        [HttpGet("order-details/{id}")]
        public IActionResult OrderDetails(string id)
        {
            ViewData["Title"] = localizer["orders_details_title"] + " #" + id;

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
                        ImageUrl = "https://images.unsplash.com/photo-1605648916319-cf082f7524a1?q=80&w=400&auto=format&fit=crop"
                    },
                    new OrderItemViewModel {
                        ProductName = "Pixel Heart Sticker",
                        Sku = "STK-PIXL-03",
                        Dimensions = "4x4 cm",
                        Quantity = 1,
                        UnitPrice = 80.00m,
                        ImageUrl = "https://images.unsplash.com/photo-1599305090598-fe179d501c27?q=80&w=400&auto=format&fit=crop"
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
            ViewData["Title"] = localizer["nav_custom_order"];
            return View();
        }

        [HttpGet("cart")]
        public IActionResult Cart()
        {
            ViewData["Title"] = localizer["nav_cart"];
            return View();
        }

        [HttpGet("checkout")]
        public IActionResult Checkout()
        {
            ViewData["Title"] = localizer["checkout_h1"];
            return View();
        }

        [HttpGet("confirmation")]
        public IActionResult OrderConfirmation(string orderId = "DK-9021")
        {
            ViewData["Title"] = localizer["conf_h1"];
            ViewData["OrderId"] = orderId;
            return View();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            ViewData["Title"] = localizer["nav_profile"];
            UserSettingsDto settings;
            try
            {
                settings = await mediator.Send(new GetUserSettingsQuery());
            }
            catch
            {
                settings = new UserSettingsDto();
            }

            return View(new CustomerProfileViewModel
            {
                FullName = settings.FullName,
                Email = settings.Email,
                ProfilePictureUrl = settings.ProfilePictureUrl
            });
        }

        [HttpPost("profile/update-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
                return BadRequest(new { success = false, message = localizer[LocalizationKeys.UploadFileMessages.Requried.Value] });

            var result = await mediator.Send(new UpdateUserSettingsCommand(
                null, null, null, null, null, null,
                ProfileImage: profileImage));

            return Json(new
            {
                success = true,
                message = localizer[LocalizationKeys.ProfileImageMessages.UploadSuccess.Value].ToString(),
                imageUrl = !string.IsNullOrEmpty(result.ProfilePictureUrl)
                    ? $"/files/{result.ProfilePictureUrl}"
                    : string.Empty
            });
        }

        [HttpPost("profile/remove-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfileImage()
        {
            await mediator.Send(new UpdateUserSettingsCommand(
                null, null, null, null, null, null,
                RemoveProfileImage: true));

            return Json(new
            {
                success = true,
                message = localizer[LocalizationKeys.ProfileImageMessages.RemoveSuccess.Value].ToString()
            });
        }

        [HttpGet("support")]
        public IActionResult Support()
        {
            ViewData["Title"] = localizer["nav_support"];
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
            ViewData["Title"] = localizer["supp_new_ticket"];
            return View(new NewTicketViewModel { OrderId = orderId });
        }

        [HttpPost("support/new")]
        public IActionResult CreateTicket(NewTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = localizer["supp_new_ticket"];
                return View("NewTicket", model);
            }

            // Mock saving ticket
            TempData["SuccessMessage"] = "Your support ticket has been created successfully!";
            return RedirectToAction("Support");
        }
    }
}
