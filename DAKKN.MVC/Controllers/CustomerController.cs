using DAKKN.Application.Common.Models;
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
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Favorites.Queries.GetFavorites;
using DAKKN.Application.Features.Favorites.Commands.ToggleFavorite;
using DAKKN.Application.Features.Favorites.Commands.RemoveFavorite;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates;
using DAKKN.Application.Features.Orders.Commands.PlaceOrder;
using DAKKN.MVC.ViewModels.Landing;

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
        public async Task<IActionResult> Products(string? searchTerm, Guid? categoryId, int pageNumber = 1, int pageSize = 12)
        {
            ViewData["Title"] = localizer["nav_shop"];
            var productsResult = await mediator.Send(new GetProductsQuery(searchTerm, categoryId, pageNumber, pageSize));
            var categories = await mediator.Send(new GetCategoriesQuery());

            var favoriteIds = await mediator.Send(new GetFavoritesQuery());
            var favSet = favoriteIds.Select(f => f.Id).ToHashSet();

            var viewModel = new ProductsViewModel
            {
                Products = productsResult.Items.ToList(),
                Categories = categories,
                PageNumber = productsResult.PageNumber,
                TotalPages = productsResult.TotalPages,
                HasPreviousPage = productsResult.HasPreviousPage,
                HasNextPage = productsResult.HasNextPage,
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                FavoriteProductIds = favSet
            };
            return View(viewModel);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            try
            {
                var product = await mediator.Send(new GetProductByIdQuery(id));
                ViewData["Title"] = product.Name;

                var favoriteIds = await mediator.Send(new GetFavoritesQuery());
                ViewData["IsFavorited"] = favoriteIds.Any(f => f.Id == id);

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
        public async Task<IActionResult> Favorites()
        {
            ViewData["Title"] = localizer["nav_favorites"];
            var products = await mediator.Send(new GetFavoritesQuery());
            var viewModel = new FavoritesViewModel
            {
                FavoriteProducts = products
            };
            return View(viewModel);
        }

        [HttpPost("favorites/toggle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteRequest request)
        {
            try
            {
                var isFavorited = await mediator.Send(new ToggleFavoriteCommand(request.ProductId));
                return Json(new { success = true, isFavorited });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("favorites/remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFavorite([FromBody] RemoveFavoriteRequest request)
        {
            try
            {
                await mediator.Send(new RemoveFavoriteCommand(request.ProductId));
                return Json(new { success = true });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = localizer["fav_not_found"] });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        public async Task<IActionResult> Cart()
        {
            ViewData["Title"] = localizer["nav_cart"];
            var cart = await mediator.Send(new GetCartQuery());
            var governorates = await mediator.Send(new GetActiveShippingGovernoratesQuery());
            var viewModel = new GuestCartViewModel
            {
                Items = cart.Items,
                ShippingGovernorateId = cart.ShippingGovernorateId,
                GovernorateName = cart.GovernorateName,
                GovernorateArName = cart.GovernorateArName,
                ShippingPrice = cart.ShippingPrice,
                Governorates = governorates
            };
            return View(viewModel);
        }

        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout()
        {
            ViewData["Title"] = localizer["checkout_h1"];
            var cart = await mediator.Send(new GetCartQuery());
            var governorates = await mediator.Send(new GetActiveShippingGovernoratesQuery());
            var viewModel = new GuestCartViewModel
            {
                Items = cart.Items,
                ShippingGovernorateId = cart.ShippingGovernorateId,
                GovernorateName = cart.GovernorateName,
                GovernorateArName = cart.GovernorateArName,
                ShippingPrice = cart.ShippingPrice,
                Governorates = governorates
            };
            return View(viewModel);
        }

        [HttpPost("place-order")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            try
            {
                var result = await mediator.Send(new PlaceOrderCommand());
                TempData["SuccessMessage"] = localizer["Order placed successfully!"];
                return RedirectToAction(nameof(OrderConfirmation), new { orderId = result.OrderNumber });
            }
            catch (BadRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Cart));
            }
            catch (ValidationException ex)
            {
                foreach (var kvp in ex.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
                return RedirectToAction(nameof(Cart));
            }
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

            TempData["SuccessMessage"] = "Your support ticket has been created successfully!";
            return RedirectToAction("Support");
        }
    }

    public record ToggleFavoriteRequest(Guid ProductId);
    public record RemoveFavoriteRequest(Guid ProductId);
}
