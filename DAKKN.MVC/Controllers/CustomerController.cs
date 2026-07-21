using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;
using DAKKN.Application.Common.Exceptions;
using DAKKN.MVC.ViewModels.Customer;
using DAKKN.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using Microsoft.Extensions.Localization;
using DAKKN.Application.Localization;
using MediatR;
using DAKKN.Application.Features.AccountSecurity.Commands.ChangePassword;
using DAKKN.Application.Features.AccountSecurity.Commands.DeleteAccount;
using DAKKN.Application.Features.Orders.Queries.GetCustomerOrders;
using DAKKN.Application.Features.Orders.Queries.GetOrderDetails;
using DAKKN.Application.Features.Orders.Commands.PlaceOrder;
using DAKKN.Application.Features.Orders.Commands.CancelOrder;
using DAKKN.Application.Features.Orders.Commands.DeleteOrder;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.Products.Queries.GetMostOrderedProducts;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Products.Queries.GetRelatedProducts;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Favorites.Queries.GetFavorites;
using DAKKN.Application.Features.StickerSuggestions.Queries.GetMySuggestions;
using DAKKN.Application.Features.Favorites.Commands.ToggleFavorite;
using DAKKN.Application.Features.Favorites.Commands.RemoveFavorite;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates;
using DAKKN.MVC.ViewModels.Landing;
using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Application.Features.BrandReviews.Queries.GetCustomerBrandReviews;
using DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.MVC.Helpers;
using System.Security.Claims;

namespace DAKKN.MVC.Controllers
{
    [RoleAuthorize(UserType.User, UserType.Admin)]
    [Route("customer")]
    public class CustomerController(IStringLocalizer<Messages> localizer, IMediator mediator) : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = localizer["Dashboard_Welcome"];

            var products = await mediator.Send(new GetMostOrderedProductsQuery(8));
            var dashboard = new DashboardDto
            {
                Recommendations = products
            };

            // Landing page hero banner
            var cmsSettings = await mediator.Send(new GetLandingPageSettingsQuery());
            ViewData["HeroSettings"] = HeroHelper.Deserialize(cmsSettings.Hero);
            ViewData["HasCmsHeroData"] = cmsSettings.Hero != "{}" && !string.IsNullOrEmpty(cmsSettings.Hero);

            var userId = GetUserId();
            if (userId != Guid.Empty)
            {
                var reviews = await mediator.Send(new GetCustomerBrandReviewsQuery(userId));
                ViewData["RecentReviews"] = reviews.OrderByDescending(r => r.CreatedAt).Take(3).ToList();

                var orders = await mediator.Send(new GetCustomerOrdersQuery());
                dashboard.TotalOrders = orders.Count;
                dashboard.LastOrder = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    ItemCount = o.ItemCount,
                    OrderDate = o.CreatedAt,
                    TotalAmount = o.TotalAmount
                }).FirstOrDefault();

                var favorites = await mediator.Send(new GetFavoritesQuery());
                dashboard.TotalFavorites = favorites.Count;

                var suggestions = await mediator.Send(new GetMySuggestionsQuery(1, 2));
                dashboard.Suggestions = suggestions.Items.ToList();
            }

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

                var related = await mediator.Send(new GetRelatedProductsQuery(id, product.CategoryId));

                return View(new ProductDetailsViewModel { Product = product, RelatedProducts = related });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        //[HttpGet("orders")]
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
                return Json(new { success = false, message = localizer[LocalizationKeys.Favorites.NotFound.Value] });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders()
        {
            ViewData["Title"] = localizer["nav_orders_title"];

            var orders = await mediator.Send(new GetCustomerOrdersQuery());

            var viewModel = new CustomerOrderListViewModel
            {
                Orders = orders.Select(o => new CustomerOrderItemViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    TrackingNumber = o.TrackingNumber,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.ItemCount,
                    TotalAmount = o.TotalAmount
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet("order-details/{id}")]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            ViewData["Title"] = localizer["orders_details_title"] + " #" + id.ToString("N")[..8].ToUpper();

            try
            {
                var order = await mediator.Send(new GetOrderDetailsQuery(id));

                var viewModel = new CustomerOrderDetailsViewModel
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    TrackingNumber = order.TrackingNumber,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    ShippingAddress = order.ShippingAddress,
                    ShippingGovernorate = order.ShippingGovernorateName,
                    ShippingPhone = order.CustomerPhone,
                    ShippingCost = order.ShippingCost,
                    Subtotal = order.Subtotal,
                    TotalAmount = order.TotalAmount,
                    Notes = order.Notes,
                    Items = order.Items.Select(i => new CustomerOrderItemDetailViewModel
                    {
                        ProductName = i.ProductName,
                        ProductImageUrl = i.ProductImageUrl,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList(),
                    StatusHistory = order.StatusHistory.Select(h => new CustomerOrderStatusHistoryViewModel
                    {
                        Status = h.NewStatus,
                        ChangedBy = h.ChangedBy,
                        ChangedAt = h.ChangedAt
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("order-details/{id}/cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest? request)
        {
            try
            {
                await mediator.Send(new CancelOrderCommand(id, request?.Reason));
                return Json(new { success = true, message = localizer[LocalizationKeys.OrderMessages.Cancelled.Value].Value });
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, message = localizer[LocalizationKeys.OrderMessages.NotFound.Value].Value });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (UnAuthorizedException)
            {
                return Json(new { success = false, message = localizer[LocalizationKeys.AuthMessages.AccessDeniedMessage.Value].Value });
            }
        }

        [HttpPost("order-details/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                await mediator.Send(new DeleteOrderCommand(id));
                TempData["SuccessMessage"] = localizer[LocalizationKeys.OrderMessages.Deleted.Value].Value;
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Orders));
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
        public async Task<IActionResult> PlaceOrder([FromForm] PlaceOrderRequest request)
        {
            try
            {
                var command = new PlaceOrderCommand(
                    request.CustomerName,
                    request.CustomerPhone,
                    request.ShippingAddress,
                    request.ShippingGovernorateId,
                    request.Notes);

                var result = await mediator.Send(command);
                TempData["SuccessMessage"] = localizer[LocalizationKeys.OrderMessages.Created.Value].Value;
                return RedirectToAction(nameof(OrderConfirmation), new { orderId = result.OrderId });
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
                return RedirectToAction(nameof(Checkout));
            }
        }

        [HttpGet("confirmation/{orderId}")]
        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            ViewData["Title"] = localizer["order_placed_title"];

            try
            {
                var order = await mediator.Send(new GetOrderDetailsQuery(orderId));
                return View(order);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
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
                null, null, null, null, null, ProfileImage: profileImage));

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

        [HttpPost("profile/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) && string.IsNullOrWhiteSpace(request.LastName))
                return Json(new { success = false, message = localizer[LocalizationKeys.ValidationMessages.Required.Value] });

            var fullName = $"{request.FirstName?.Trim() ?? ""} {request.LastName?.Trim() ?? ""}".Trim();

            var result = await mediator.Send(new UpdateUserSettingsCommand(fullName, null, null, null, null, null));

            return Json(new
            {
                success = true,
                message = localizer[LocalizationKeys.ProfileImageMessages.UploadSuccess.Value].ToString(),
                fullName = result.FullName
            });
        }

        [HttpPost("profile/change-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await mediator.Send(new ChangePasswordCommand(
                    request.CurrentPassword,
                    request.NewPassword,
                    request.ConfirmPassword));

                return Json(new
                {
                    success = true,
                    message = localizer[LocalizationKeys.Profile.PasswordChanged.Value].ToString()
                });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.SelectMany(e => e.Value);
                return Json(new { success = false, message = string.Join(" ", errors) });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = localizer[LocalizationKeys.Profile.PasswordChangeError.Value].ToString() });
            }
        }

        [HttpPost("profile/delete-account")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            try
            {
                await mediator.Send(new DeleteAccountCommand(
                    request.CurrentPassword,
                    request.ConfirmationText));

                await HttpContext.SignOutAsync();

                return Json(new
                {
                    success = true,
                    message = localizer[LocalizationKeys.Profile.DeleteAccountSuccess.Value].ToString(),
                    redirectUrl = Url.Action("Index", "Home")
                });
            }
            catch (BadRequestException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.SelectMany(e => e.Value);
                return Json(new { success = false, message = string.Join(" ", errors) });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = localizer[LocalizationKeys.Profile.DeleteAccountError.Value].ToString() });
            }
        }

        [HttpGet("brand-reviews")]
        public async Task<IActionResult> BrandReviews()
        {
            ViewData["Title"] = localizer["brand_reviews_my_reviews"];
            var userId = GetUserId();
            var reviews = await mediator.Send(new GetCustomerBrandReviewsQuery(userId));
            return View(new CustomerBrandReviewsViewModel { Reviews = reviews });
        }

        [HttpPost("brand-reviews/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrandReview(Guid id)
        {
            var userId = GetUserId();
            await mediator.Send(new DeleteBrandReviewCommand(id, userId));
            TempData["SuccessMessage"] = localizer[LocalizationKeys.BrandReviews.Deleted.Value].Value;
            return RedirectToAction("BrandReviews");
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    public record ToggleFavoriteRequest(Guid ProductId);
    public record RemoveFavoriteRequest(Guid ProductId);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
    public record DeleteAccountRequest(string CurrentPassword, string ConfirmationText);
    public record CancelOrderRequest(string? Reason);
    public record UpdateProfileRequest(string? FirstName, string? LastName);
}
