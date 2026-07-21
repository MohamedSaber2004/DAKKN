using DAKKN.Appearence.Filters;
using DAKKN.Appearence.Services;
using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrders;
using DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrderById;
using DAKKN.Application.Features.CustomOrders.Commands.ApproveCustomOrder;
using DAKKN.Application.Features.CustomOrders.Commands.RejectCustomOrder;
using DAKKN.Application.Features.Categories.Commands.CreateCategory;
using DAKKN.Application.Features.Categories.Commands.DeleteCategory;
using DAKKN.Application.Features.Categories.Commands.UpdateCategory;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Attachments.Commands.UpdateImage;
using DAKKN.Application.Features.Products.Commands.CreateProduct;
using DAKKN.Application.Features.Products.Commands.DeleteProduct;
using DAKKN.Application.Features.Products.Commands.UpdateProduct;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.ShippingGovernorates.Commands.CreateShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Commands.DeleteShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Commands.ToggleShippingGovernorateStatus;
using DAKKN.Application.Features.ShippingGovernorates.Commands.UpdateShippingGovernorate;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetShippingGovernorates;
using DAKKN.Application.Features.Dashboard.Queries.GetDashboardAnalytics;
using DAKKN.Application.Features.Dashboard.Queries.GetDashboardInventoryStats;
using DAKKN.Application.Features.Dashboard.Queries.GetRecentProductRatings;
using DAKKN.Application.Features.Orders.Queries.GetOrders;
using DAKKN.Application.Features.Orders.Queries.GetOrderDetails;
using DAKKN.Application.Features.Orders.Queries.ExportUndeliveredOrders;
using DAKKN.Application.Features.Orders.Queries.GetRecentOrders;
using DAKKN.Application.Features.Orders.Queries.GetDashboardStats;
using DAKKN.Application.Features.Orders.Commands.UpdateOrderStatus;
using DAKKN.Application.Features.Orders.Commands.CancelOrder;
using DAKKN.Application.Features.Orders.Commands.DeleteOrder;
using DAKKN.Application.Features.Inventory.Commands.ApplyGlobalDangerQuantity;
using DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings;
using DAKKN.Application.Features.Inventory.Queries.GetInventorySettings;
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using DAKKN.Application.Features.Users.Queries.ExportUsers;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using DAKKN.Application.Features.Users.Queries.GetUserStats;
using DAKKN.Application.Features.CMS.Commands.UpdateLandingPageSettings;
using DAKKN.Application.Features.CMS.DTOs;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.Application.Localization;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Enums;
using DAKKN.MVC.Helpers;
using DAKKN.MVC.ViewModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using DAKKN.Application.Features.BrandReviews.Queries.GetAdminBrandReviews;
using DAKKN.Application.Features.BrandReviews.Commands.ApproveBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.RejectBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.ToggleDisplayBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.DeleteBrandReview;
using DAKKN.Application.Features.BrandReviews.Commands.UpdateDisplayOrderBrandReview;
using System.Security.Claims;

namespace DAKKN.MVC.Controllers
{
    [Route("admin")]
    [RoleAuthorize(UserType.Admin)]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IMediator _mediator;
        private readonly IImageValidator _imageValidator;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IWebHostEnvironment env, IStringLocalizer<Messages> localizer, IMediator mediator, IImageValidator imageValidator, ILogger<AdminController> logger)
        {
            _env = env;
            _localizer = localizer;
            _mediator = mediator;
            _imageValidator = imageValidator;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            ViewData["Title"] = _localizer["admin_overview"];
            var inventoryStats = await _mediator.Send(new GetDashboardInventoryStatsQuery());
            var orderStats = await _mediator.Send(new GetDashboardStatsQuery());
            var recentOrders = await _mediator.Send(new GetRecentOrdersQuery(Count: 10));

            ViewData["LowStockCount"] = inventoryStats.LowStockCount;
            ViewData["OutOfStockCount"] = inventoryStats.OutOfStockCount;
            ViewData["TotalProducts"] = inventoryStats.TotalProducts;

            var viewModel = new DashboardStatsViewModel
            {
                OrdersToday = orderStats.OrdersToday,
                OrdersLast24Hours = orderStats.OrdersLast24Hours,
                RevenueToday = orderStats.RevenueToday,
                RevenueLast24Hours = orderStats.RevenueLast24Hours,
                PendingOrders = orderStats.PendingOrders,
                DeliveredOrders = orderStats.DeliveredOrders,
                CancelledOrders = orderStats.CancelledOrders,
                TotalProducts = orderStats.TotalProducts,
                TotalUsers = orderStats.TotalUsers
            };

            var analytics7d = await _mediator.Send(new GetDashboardAnalyticsQuery(Days: 7));
            var analytics30d = await _mediator.Send(new GetDashboardAnalyticsQuery(Days: 30));

            ViewData["RecentOrders"] = recentOrders.Select(o => new RecentOrderWidgetViewModel
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.CustomerName,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            }).ToList();

            ViewData["Analytics7d"] = analytics7d;
            ViewData["Analytics30d"] = analytics30d;

            var pendingReviews = await _mediator.Send(new GetAdminBrandReviewsQuery(StatusFilter: "pending", null, null, null));
            ViewData["PendingReviewCount"] = pendingReviews.Count;

            var recentRatings = await _mediator.Send(new GetRecentProductRatingsQuery(Count: 5));
            ViewData["RecentProductRatings"] = recentRatings;

            return View(viewModel);
        }

        [HttpGet("users")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Users(string? searchTerm, string? role, string? status, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = _localizer["admin_users"];

            var query = new GetAllUsersQuery(searchTerm, role, status, pageNumber, pageSize);
            var usersResult = await _mediator.Send(query);
            var userViewModels = usersResult.Items.Select(u => new UserListItemViewModel
            {
                Id = u.Id.ToString(),
                Name = u.FullName,
                Email = u.Email,
                Phone = u.PhoneNumber ?? "",
                JoinDate = u.JoinDate,
                AvatarUrl = !string.IsNullOrEmpty(u.ProfilePictureUrl)
                    ? u.ProfilePictureUrl.StartsWith("http") || u.ProfilePictureUrl.StartsWith("/")
                        ? u.ProfilePictureUrl
                        : $"/files/{u.ProfilePictureUrl}"
                    : $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(u.FullName ?? "User")}&background=random",
                Role = u.Roles.Contains("Admin") ? UserRole.Admin : UserRole.Customer,
                Status = u.Status switch
                {
                    "Active" => UserStatus.Active,
                    "Deleted" => UserStatus.Deleted,
                    _ => UserStatus.Active
                }
            }).ToList();

            var stats = await _mediator.Send(new GetUserStatsQuery());

            var viewModel = new UserManagementViewModel
            {
                Users = userViewModels,
                TotalUsers = stats.TotalUsers,
                ActiveUsers = stats.ActiveUsers,
                DeletedUsers = stats.DeletedUsers,
                PageNumber = usersResult.PageNumber,
                PageSize = usersResult.PageSize,
                TotalPages = usersResult.TotalPages,
                HasPreviousPage = usersResult.HasPreviousPage,
                HasNextPage = usersResult.HasNextPage
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UserListPartial", viewModel);
            }

            return View(viewModel);
        }
        [HttpGet("users/export")]
        public async Task<IActionResult> ExportUsers(string? searchTerm, string? role, string? status)
        {
            var query = new ExportUsersQuery(searchTerm, role, status);
            var users = await _mediator.Send(query);

            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Full Name,Email,Phone,Join Date,Role,Status");

            foreach (var user in users)
            {
                builder.AppendLine($"\"{user.FullName}\",\"{user.Email}\",\"{user.PhoneNumber}\",\"{user.JoinDate:yyyy-MM-dd}\",\"{string.Join("|", user.Roles)}\",\"{user.Status}\"");
            }

            var csvData = System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(builder.ToString())).ToArray();
            return File(csvData, "text/csv", $"users_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        [HttpGet("orders/export-undelivered-pdf")]
        public async Task<IActionResult> ExportUndeliveredOrdersPdf()
        {
            var orders = await _mediator.Send(new ExportUndeliveredOrdersQuery());
            var pdfService = HttpContext.RequestServices.GetRequiredService<IPdfExportService>();
            var pdfBytes = pdfService.GenerateUndeliveredOrdersPdf(orders);
            return File(pdfBytes, "application/pdf", $"undelivered_orders_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpGet("custom-orders/export-undelivered-pdf")]
        public async Task<IActionResult> ExportUndeliveredCustomOrdersPdf()
        {
            var orders = await _mediator.Send(new DAKKN.Application.Features.CustomOrders.Queries.ExportUndeliveredCustomOrders.ExportUndeliveredCustomOrdersQuery());
            var pdfService = HttpContext.RequestServices.GetRequiredService<IPdfExportService>();
            var pdfBytes = pdfService.GenerateUndeliveredCustomOrdersPdf(orders);
            return File(pdfBytes, "application/pdf", $"pending_custom_orders_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(string? searchTerm, OrderStatus? filterStatus, int page = 1)
        {
            ViewData["Title"] = _localizer["admin_orders"];

            var result = await _mediator.Send(new GetOrdersQuery(searchTerm, filterStatus, Page: page));

            var viewModel = new AdminOrderListViewModel
            {
                Orders = result.Orders.Select(o => new OrderListItemViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.CustomerName,
                    CreatedAt = o.CreatedAt,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    TrackingNumber = o.TrackingNumber,
                    ItemCount = o.ItemCount
                }).ToList(),
                TotalOrders = result.TotalCount,
                PendingCount = result.PendingCount,
                ProcessingCount = result.ProcessingCount,
                ShippedCount = result.ShippedCount,
                DeliveredCount = result.DeliveredCount,
                CancelledCount = result.CancelledCount,
                MonthlyRevenue = result.MonthlyRevenue,
                SearchTerm = searchTerm,
                FilterStatus = filterStatus
            };

            return View(viewModel);
        }

        [HttpGet("order-details/{id}")]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            ViewData["Title"] = _localizer["admin_order_details"];

            try
            {
                var order = await _mediator.Send(new GetOrderDetailsQuery(id, IsAdmin: true));

                var viewModel = new AdminOrderDetailsViewModel
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    TrackingNumber = order.TrackingNumber,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    CustomerName = order.CustomerName,
                    CustomerEmail = order.CustomerEmail,
                    CustomerPhone = order.CustomerPhone,
                    ShippingAddress = order.ShippingAddress,
                    ShippingGovernorateName = order.ShippingGovernorateName,
                    ShippingCost = order.ShippingCost,
                    Subtotal = order.Subtotal,
                    TotalAmount = order.TotalAmount,
                    Notes = order.Notes,
                    Items = order.Items.Select(i => new OrderItemViewModel
                    {
                        ProductName = i.ProductName,
                        ProductImageUrl = i.ProductImageUrl,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList(),
                    StatusHistory = order.StatusHistory.Select(h => new OrderStatusHistoryViewModel
                    {
                        OldStatus = h.OldStatus,
                        NewStatus = h.NewStatus,
                        ChangedBy = h.ChangedBy,
                        ChangedAt = h.ChangedAt,
                        Notes = h.Notes
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("order-details/{id}/update-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, OrderStatus newStatus, string? statusNotes = null)
        {
            try
            {
                await _mediator.Send(new UpdateOrderStatusCommand(id, newStatus, statusNotes));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.OrderMessages.StatusUpdated.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(OrderDetails), new { id });
        }

        [HttpPost("orders/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteOrderCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.OrderMessages.Deleted.Value].Value;
            }
            catch (Exception ex) when (ex is ValidationException or BadRequestException or NotFoundException)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Orders));
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> Inventory(string? searchTerm, Guid? categoryId, string? stockFilter, string? sortBy, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = _localizer["admin_inventory"];

            var result = await _mediator.Send(new GetProductsQuery(searchTerm, categoryId, pageNumber, pageSize, null, stockFilter, sortBy));
            var categories = await _mediator.Send(new GetCategoriesQuery());

            var viewModel = new InventoryViewModel
            {
                Products = result.Items.ToList(),
                Categories = categories,
                TotalProducts = result.TotalCount,
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm,
                PageNumber = result.PageNumber,
                TotalPages = result.TotalPages,
                StockFilter = stockFilter,
                SortBy = sortBy
            };

            var allProducts = await _mediator.Send(new GetProductsQuery(null, null, 1, 10000));
            viewModel.LowStockCount = allProducts.Items.Count(p => p.QuantityInStock > 0 && p.QuantityInStock <= p.DangerQuantity);
            viewModel.OutOfStockCount = allProducts.Items.Count(p => p.QuantityInStock == 0);

            return View(viewModel);
        }

        [HttpGet("support")]
        public IActionResult Support()
        {
            return RedirectToAction("Dashboard", "SupportAdmin");
        }

        [HttpPost("content/upload-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadContentImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = _localizer[LocalizationKeys.UploadFileMessages.Requried.Value] });

            var (uploaded, result) = await _imageValidator.UploadImage(file, 0);
            if (!uploaded)
                return Json(new { success = false, message = result });

            var url = $"/files/{result}";
            return Json(new { success = true, imageUrl = url });
        }

        [HttpGet("content")]
        public async Task<IActionResult> Content()
        {
            ViewData["Title"] = _localizer["admin_content_mgmt"];
            var settings = await _mediator.Send(new GetLandingPageSettingsQuery());
            var model = MapToViewModel(settings);

            var allProducts = await _mediator.Send(new GetProductsQuery(null, null, 1, 200));
            var allCategories = await _mediator.Send(new GetCategoriesQuery(IncludeInactive: true));

            var storedFeaturedIds = new HashSet<string>(
                (model.Products.SelectedProductIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries));

            model.Products.AllProducts = allProducts.Items.Select(p => new ProductManagementItem
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Category = p.CategoryName,
                ImageUrl = p.ImageFullUrl ?? p.ImageUrl ?? string.Empty,
                IsFeatured = storedFeaturedIds.Contains(p.Id.ToString())
            }).ToList();

            model.Categories.AllCategories = allCategories.Select(c => new CategoryManagementItem
            {
                Id = c.Id.ToString(),
                Name = c.CategoryName,
                Icon = "category",
                IsFeatured = false
            }).ToList();

            model.BrandReviews = await _mediator.Send(new GetAdminBrandReviewsQuery(null, null, null, null));

            return View(model);
        }

        [HttpPost("content/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateContent()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var incoming = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);

            var existingDto = await _mediator.Send(new GetLandingPageSettingsQuery());
            var existingVm = MapToViewModel(existingDto);

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (incoming.TryGetProperty("sectionOrder", out var sectionOrder))
                existingVm.SectionOrder = System.Text.Json.JsonSerializer.Deserialize<SectionOrderViewModel>(sectionOrder.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("hero", out var hero))
                existingVm.Hero = System.Text.Json.JsonSerializer.Deserialize<HeroSettingsViewModel>(hero.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("about", out var about))
                existingVm.About = System.Text.Json.JsonSerializer.Deserialize<AboutSettingsViewModel>(about.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("categories", out var categories))
                existingVm.Categories = System.Text.Json.JsonSerializer.Deserialize<FeaturedCategoriesViewModel>(categories.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("products", out var products))
                existingVm.Products = System.Text.Json.JsonSerializer.Deserialize<FeaturedProductsViewModel>(products.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("testimonials", out var testimonials))
                existingVm.Testimonials = System.Text.Json.JsonSerializer.Deserialize<TestimonialsSettingsViewModel>(testimonials.GetRawText(), options) ?? new();
            if (incoming.TryGetProperty("contact", out var contact))
                existingVm.Contact = System.Text.Json.JsonSerializer.Deserialize<ContactSettingsViewModel>(contact.GetRawText(), options) ?? new();

            var settings = MapToDto(existingVm);
            await _mediator.Send(settings);
            return Json(new { success = true });
        }

        [HttpGet("brand-reviews")]
        public async Task<IActionResult> BrandReviews(string? statusFilter, int? ratingFilter, string? searchTerm, string? sortBy)
        {
            ViewData["Title"] = _localizer["brand_reviews_admin_title"];
            ViewData["currentAction"] = "BrandReviews";

            var query = new GetAdminBrandReviewsQuery(statusFilter, ratingFilter, searchTerm, sortBy);
            var reviews = await _mediator.Send(query);

            var vm = new AdminBrandReviewsViewModel
            {
                Reviews = reviews,
                StatusFilter = statusFilter,
                RatingFilter = ratingFilter,
                SearchTerm = searchTerm,
                SortBy = sortBy
            };

            return View(vm);
        }

        [HttpPost("brand-reviews/toggle-display/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleDisplayBrandReview(Guid id, string? returnUrl)
        {
            try
            {
                await _mediator.Send(new ToggleDisplayBrandReviewCommand(id));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Redirect(returnUrl ?? "/admin/brand-reviews");
        }

        [HttpPost("brand-reviews/approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBrandReview(Guid id, string? returnUrl)
        {
            try
            {
                var adminId = GetUserId();
                await _mediator.Send(new ApproveBrandReviewCommand(id, adminId));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Redirect(returnUrl ?? "/admin/brand-reviews");
        }

        [HttpPost("brand-reviews/reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBrandReview(Guid id, string? returnUrl)
        {
            try
            {
                await _mediator.Send(new RejectBrandReviewCommand(id));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Redirect(returnUrl ?? "/admin/brand-reviews");
        }

        [HttpPost("brand-reviews/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBrandReview(Guid id, string? returnUrl)
        {
            var userId = GetUserId();
            await _mediator.Send(new DeleteBrandReviewCommand(id, userId));
            return Redirect(returnUrl ?? "/admin/brand-reviews");
        }

        [HttpPost("brand-reviews/update-display-order/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDisplayOrderBrandReview(Guid id, [FromForm] UpdateDisplayOrderBrandReviewCommand command)
        {
            try
            {
                command = command with { Id = id };
                await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return Redirect("/admin/brand-reviews");
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private static ContentManagementViewModel MapToViewModel(LandingPageSettingsDto dto)
        {
            return new ContentManagementViewModel
            {
                SectionOrder = DeserializeOrDefault<SectionOrderViewModel>(dto.SectionOrder),
                Hero =         HeroHelper.Deserialize(dto.Hero),
                About =        AboutHelper.Deserialize(dto.About),
                Categories =   DeserializeOrDefault<FeaturedCategoriesViewModel>(dto.Categories),
                Products =     DeserializeOrDefault<FeaturedProductsViewModel>(dto.Products),
                Testimonials = DeserializeOrDefault<TestimonialsSettingsViewModel>(dto.Testimonials),
                Contact =      DeserializeOrDefault<ContactSettingsViewModel>(dto.Contact),
                Faq =          DeserializeOrDefault<FaqSettingsViewModel>(dto.Faq),
            };
        }

        private static UpdateLandingPageSettingsCommand MapToDto(ContentManagementViewModel model)
        {
            return new UpdateLandingPageSettingsCommand
            {
                SectionOrder = System.Text.Json.JsonSerializer.Serialize(model.SectionOrder),
                Hero =         System.Text.Json.JsonSerializer.Serialize(model.Hero),
                About =        System.Text.Json.JsonSerializer.Serialize(model.About),
                Categories =   System.Text.Json.JsonSerializer.Serialize(model.Categories),
                Products =     System.Text.Json.JsonSerializer.Serialize(model.Products),
                Testimonials = System.Text.Json.JsonSerializer.Serialize(model.Testimonials),
                Contact =      System.Text.Json.JsonSerializer.Serialize(model.Contact),
                Faq =          System.Text.Json.JsonSerializer.Serialize(model.Faq),
            };
        }

        private static T DeserializeOrDefault<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json) || json == "{}" || json == "[]")
                return new T();
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? new T();
            }
            catch
            {
                return new T();
            }
        }

        [HttpGet("custom-orders")]
        public async Task<IActionResult> CustomOrders(CustomOrderStatus? filterStatus, int page = 1)
        {
            ViewData["Title"] = _localizer["admin_custom_order.title"];

            try
            {
                var result = await _mediator.Send(new GetCustomOrdersQuery(filterStatus, page));

                var viewModel = new AdminCustomOrderListViewModel
                {
                    Orders = result.Orders.Select(o => new AdminCustomOrderItemViewModel
                    {
                        Id = o.Id,
                        CustomerName = o.CustomerName,
                        CustomerPhone = o.CustomerPhone,
                        ImageUrl = o.ImageUrl,
                        Quantity = o.Quantity,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt
                    }).ToList(),
                    TotalCount = result.TotalCount,
                    PendingCount = result.PendingCount,
                    ApprovedCount = result.ApprovedCount,
                    RejectedCount = result.RejectedCount,
                    FilterStatus = filterStatus,
                    Page = page
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading custom orders page");
                return Content($"Error: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}", "text/plain");
            }
        }

        [HttpGet("custom-order-details/{id}")]
        public async Task<IActionResult> CustomOrderDetails(Guid id)
        {
            ViewData["Title"] = _localizer["admin_custom_order.details"];

            try
            {
                var order = await _mediator.Send(new GetCustomOrderByIdQuery(id));
                if (order == null)
                    return NotFound();

                var viewModel = new AdminCustomOrderDetailsViewModel
                {
                    Id = order.Id,
                    CustomerName = order.CustomerName,
                    CustomerPhone = order.CustomerPhone,
                    ShippingAddress = order.ShippingAddress,
                    Notes = order.Notes,
                    ImageUrl = order.ImageUrl,
                    Shape = order.Shape,
                    Size = order.Size,
                    Quantity = order.Quantity,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                };

                return View(viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("custom-orders/{id}/approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCustomOrder(Guid id)
        {
            try
            {
                await _mediator.Send(new ApproveCustomOrderCommand(id));
                TempData["Success"] = _localizer["admin_custom_order.approved"];
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(CustomOrderDetails), new { id });
        }

        [HttpPost("custom-orders/{id}/reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCustomOrder(Guid id)
        {
            try
            {
                await _mediator.Send(new RejectCustomOrderCommand(id));
                TempData["Success"] = _localizer["admin_custom_order.rejected"];
            }
            catch (NotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(CustomOrderDetails), new { id });
        }

        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            ViewData["Title"] = _localizer["admin_settings"];

            UserSettingsDto? settings = null;
            InventorySettingsDto? inventorySettings = null;

            try
            {
                settings = await _mediator.Send(new GetUserSettingsQuery());
            }
            catch (UnAuthorizedException)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user settings for admin settings page");
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ExceptionMessages.UnknownException.Value].ToString();
                settings = new UserSettingsDto
                {
                    FullName = User.FindFirstValue("FullName") ?? User.Identity?.Name ?? "Admin",
                    Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty
                };
            }

            try
            {
                inventorySettings = await _mediator.Send(new GetInventorySettingsQuery());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load inventory settings for admin settings page");
            }

            var viewModel = new AdminSettingsViewModel
            {
                FullName          = settings.FullName,
                Email             = settings.Email,
                Language          = settings.Language,
                Theme             = settings.Theme,
                IsDarkMode        = settings.IsDarkMode,
                LayoutMode        = settings.LayoutMode,
                ProfilePictureUrl = settings.ProfilePictureUrl
            };

            ViewData["GlobalDangerQuantity"] = inventorySettings?.GlobalDangerQuantity ?? 10;

            return View(viewModel);
        }

        [HttpPost("settings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Settings(AdminSettingsViewModel model, int globalDangerQuantity = 10)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_settings"];
                try
                {
                    var inventorySettings = await _mediator.Send(new GetInventorySettingsQuery());
                    ViewData["GlobalDangerQuantity"] = inventorySettings.GlobalDangerQuantity;
                }
                catch
                {
                    ViewData["GlobalDangerQuantity"] = 10;
                }
                return View(model);
            }

            // Get current settings to check if language changed
            UserSettingsDto currentSettings;
            try
            {
                currentSettings = await _mediator.Send(new GetUserSettingsQuery());
            }
            catch
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login", "Auth");
            }

            await _mediator.Send(new UpdateUserSettingsCommand(
                model.FullName,
                model.Language,
                model.Theme,
                model.IsDarkMode,
                model.LayoutMode
            ));

            // Save inventory settings and apply to all products
            await _mediator.Send(new UpdateInventorySettingsCommand(globalDangerQuantity));
            var updatedCount = await _mediator.Send(new ApplyGlobalDangerQuantityCommand());

            TempData["SuccessMessage"] = string.Format(
                _localizer[LocalizationKeys.Inventory.GlobalDangerApplied.Value].ToString(), updatedCount);

            // If language changed via form, redirect through SetLanguage to update cookie
            if (model.Language != null && currentSettings.Language != model.Language)
            {
                return RedirectToAction("SetLanguage", "Translation", new { 
                    culture = model.Language, 
                    returnUrl = Url.Action(nameof(Settings)) 
                });
            }

            return RedirectToAction(nameof(Settings));
        }

        /// <summary>AJAX – upload/replace profile picture.</summary>
        [HttpPost("settings/update-profile-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
                return BadRequest(new { success = false, message = _localizer[LocalizationKeys.UploadFileMessages.Requried.Value] });

            var result = await _mediator.Send(new UpdateUserSettingsCommand(
                null, null, null, null, null, ProfileImage: profileImage));

            return Json(new
            {
                success = true,
                message = _localizer[LocalizationKeys.ProfileImageMessages.UploadSuccess.Value].ToString(),
                imageUrl = !string.IsNullOrEmpty(result.ProfilePictureUrl)
                    ? $"/files/{result.ProfilePictureUrl}"
                    : string.Empty
            });
        }

        /// <summary>AJAX – remove profile picture.</summary>
        [HttpPost("settings/remove-profile-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProfileImage()
        {
            await _mediator.Send(new UpdateUserSettingsCommand(
                null, null, null, null, null, null,
                RemoveProfileImage: true));

            return Json(new
            {
                success = true,
                message = _localizer[LocalizationKeys.ProfileImageMessages.RemoveSuccess.Value].ToString()
            });
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> CategoryProducts(Guid id, int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _mediator.Send(new GetCategoriesQuery(IncludeInactive: true));
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return View("NotFound");

            ViewData["Title"] = category.CategoryName;

            var result = await _mediator.Send(new GetProductsQuery(null, id, pageNumber, pageSize));
            var activeProducts = result.Items.Count(p => p.IsActive);
            var avgPrice = result.Items.Any() ? result.Items.Average(p => p.Price) : 0;

            var viewModel = new CategoryProductsViewModel
            {
                CategoryId = category.Id,
                CategoryName = category.CategoryName,
                ArName = category.ArName,
                ProductsCount = category.ProductsCount,
                Products = result.Items.ToList(),
                AveragePrice = avgPrice,
                ActiveProducts = activeProducts,
                PageNumber = result.PageNumber,
                TotalPages = result.TotalPages
            };

            return View(viewModel);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Categories(string? searchTerm)
        {
            ViewData["Title"] = _localizer["admin_categories"];
            ViewData["SearchTerm"] = searchTerm;
            var categories = await _mediator.Send(new GetCategoriesQuery(searchTerm, IncludeInactive: true));
            return View(categories);
        }

        [HttpGet("categories/create")]
        public IActionResult CreateCategory()
        {
            ViewData["Title"] = _localizer["admin_categories_add_new"];
            return View("AddCategory", new CategoryFormViewModel());
        }

        [HttpPost("categories/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_categories_add_new"];
                return View("AddCategory", model);
            }

            try
            {
                string? imageUrl = null;
                if (model.ImageFile != null)
                {
                    var result = await _mediator.Send(new UpdateImageCommand
                    {
                        File = model.ImageFile,
                        UploadPlace = 2
                    });
                    imageUrl = result;
                }

                await _mediator.Send(new CreateCategoryCommand(model.CategoryName, model.ArName, imageUrl));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Categories.Created.Value].ToString();
                return RedirectToAction(nameof(Categories));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer["admin_categories_add_new"];
            return View("AddCategory", model);
        }

        [HttpGet("categories/edit/{id}")]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            ViewData["Title"] = _localizer["admin_categories_edit"];

            var categories = await _mediator.Send(new GetCategoriesQuery(IncludeInactive: true));
            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            var viewModel = new CategoryFormViewModel
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                ArName = category.ArName,
                ExistingImageUrl = category.ImageUrl
            };

            return View("AddCategory", viewModel);
        }

        [HttpPost("categories/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Guid id, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_categories_edit"];
                return View("AddCategory", model);
            }

            try
            {
                var rawCategoryUrl = model.ExistingImageUrl;
                string? imageUrl = string.IsNullOrEmpty(rawCategoryUrl) ? null
                    : rawCategoryUrl.StartsWith("http")
                        ? rawCategoryUrl
                        : Path.GetFileName(rawCategoryUrl.TrimEnd('/'));

                if (model.ImageFile != null)
                {
                    var oldFileName = string.IsNullOrEmpty(rawCategoryUrl) || rawCategoryUrl.StartsWith("http")
                        ? null
                        : Path.GetFileName(rawCategoryUrl.TrimEnd('/'));

                    var result = await _mediator.Send(new UpdateImageCommand
                    {
                        File = model.ImageFile,
                        UploadPlace = 2,
                        ImageName = oldFileName
                    });
                    imageUrl = result;
                }

                await _mediator.Send(new UpdateCategoryCommand(id, model.CategoryName, model.ArName, imageUrl, IsActive: true));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Categories.Updated.Value].ToString();
                return RedirectToAction(nameof(Categories));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer["admin_categories_edit"];
            return View("AddCategory", model);
        }

        [HttpPost("categories/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteCategoryCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Categories.Deleted.Value].ToString();
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.Categories.NotFound.Value].ToString();
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpPost("categories/toggle-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCategoryStatus(Guid id)
        {
            try
            {
                var categories = await _mediator.Send(new GetCategoriesQuery(IncludeInactive: true));
                var category = categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                    throw new NotFoundException("Category", id);

                await _mediator.Send(new UpdateCategoryCommand(id, category.CategoryName, category.ArName, IsActive: category.IsDeleted));
                TempData["SuccessMessage"] = _localizer[category.IsDeleted ? LocalizationKeys.Categories.Restored.Value : LocalizationKeys.Categories.Deleted.Value].ToString();
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.Categories.NotFound.Value].ToString();
            }

            return RedirectToAction(nameof(Categories));
        }

        [HttpGet("add-product")]
        public async Task<IActionResult> AddProduct()
        {
            ViewData["Title"] = _localizer["admin_add_product"];

            var categories = await _mediator.Send(new GetCategoriesQuery());
            var inventorySettings = await _mediator.Send(new GetInventorySettingsQuery());

            var viewModel = new AddProductViewModel
            {
                AvailableCategories = categories,
                DangerQuantity = inventorySettings.GlobalDangerQuantity
            };

            return View(viewModel);
        }

        [HttpPost("add-product")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_add_product"];
                model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
                return View(model);
            }

            var rawUrl = model.ExistingImageUrl;
            var imageUrl = string.IsNullOrEmpty(rawUrl)
                ? string.Empty
                : rawUrl.StartsWith("http")
                    ? rawUrl
                    : Path.GetFileName(rawUrl.TrimEnd('/'));

            if (model.ImageFile != null)
            {
                try
                {
                    var result = await _mediator.Send(new UpdateImageCommand
                    {
                        File = model.ImageFile,
                        UploadPlace = 1
                    });
                    imageUrl = result;
                }
                catch
                {
                    TempData["Error"] = _localizer[LocalizationKeys.UploadFileMessages.FileUploadFailed.Value].ToString();
                    model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
                    return View(model);
                }
            }

            var command = new CreateProductCommand(
                model.Name,
                model.ArName,
                model.Description,
                model.ArDescription,
                model.Price,
                imageUrl,
                new List<string>(),
                model.SizeOptions,
                model.CategoryId,
                model.QuantityInStock,
                model.DangerQuantity
            );

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Products.Created.Value].ToString();
                return RedirectToAction(nameof(Inventory));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer["admin_add_product"];
            model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
            return View(model);
        }

        [HttpPost("delete-product/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteProductCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Products.Deleted.Value].ToString();
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.Products.NotFound.Value].ToString();
            }

            return RedirectToAction(nameof(Inventory));
        }

        [HttpGet("product-details/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            ViewData["Title"] = _localizer["admin_product_details_title"];

            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                return View(product);
            }
            catch (NotFoundException)
            {
                return View("NotFound");
            }
        }

        [HttpGet("edit-product/{id}")]
        public async Task<IActionResult> EditProduct(Guid id)
        {
            ViewData["Title"] = _localizer["admin_product_edit_title"];

            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                var categories = await _mediator.Send(new GetCategoriesQuery());

                var viewModel = new AddProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    ArName = product.ArName,
                    Description = product.Description,
                    ArDescription = product.ArDescription,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    ExistingImageUrl = product.ImageUrl ?? string.Empty,
                    SizeOptions = product.SizeOptions,
                    QuantityInStock = product.QuantityInStock,
                    DangerQuantity = product.DangerQuantity,
                    AvailableCategories = categories
                };

                return View("AddProduct", viewModel);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("edit-product/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Guid id, AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_product_edit_title"];
                model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
                return View("AddProduct", model);
            }

            var rawUrl = model.ExistingImageUrl;
            var imageUrl = string.IsNullOrEmpty(rawUrl)
                ? string.Empty
                : rawUrl.StartsWith("http")
                    ? rawUrl
                    : Path.GetFileName(rawUrl.TrimEnd('/'));

            if (model.ImageFile != null)
            {
                try
                {
                    var result = await _mediator.Send(new UpdateImageCommand
                    {
                        File = model.ImageFile,
                        UploadPlace = 1,
                        ImageName = string.IsNullOrEmpty(imageUrl) || imageUrl.StartsWith("http") || !Path.HasExtension(imageUrl) ? null : imageUrl
                    });
                    imageUrl = result;
                }
                catch
                {
                    TempData["Error"] = _localizer[LocalizationKeys.UploadFileMessages.FileUploadFailed.Value].ToString();
                    ViewData["Title"] = _localizer["admin_product_edit_title"];
                    model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
                    return View("AddProduct", model);
                }
            }

            var command = new UpdateProductCommand(
                id,
                model.Name,
                model.ArName,
                model.Description,
                model.ArDescription,
                model.Price,
                imageUrl,
                new List<string>(),
                model.SizeOptions,
                model.CategoryId,
                model.QuantityInStock,
                model.DangerQuantity
            );

            try
            {
                await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Products.Updated.Value].ToString();
                return RedirectToAction(nameof(Inventory));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer["admin_product_edit_title"];
            model.AvailableCategories = await _mediator.Send(new GetCategoriesQuery());
            return View("AddProduct", model);
        }

        [HttpGet("inventory-settings")]
        [HttpGet("settings/inventory")]
        public async Task<IActionResult> InventorySettings()
        {
            var settings = await _mediator.Send(new GetInventorySettingsQuery());
            return Json(new { globalDangerQuantity = settings.GlobalDangerQuantity });
        }

        [HttpPost("settings/inventory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInventorySettings(int globalDangerQuantity)
        {
            try
            {
                await _mediator.Send(new UpdateInventorySettingsCommand(globalDangerQuantity));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.Inventory.Updated.Value].ToString();
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Settings));
        }

        [HttpPost("settings/inventory/apply-global")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyGlobalDangerQuantity()
        {
            var count = await _mediator.Send(new ApplyGlobalDangerQuantityCommand());
            TempData["SuccessMessage"] = string.Format(_localizer[LocalizationKeys.Inventory.GlobalDangerApplied.Value].ToString(), count);
            return RedirectToAction(nameof(Settings));
        }

        [HttpGet("shipping")]
        public async Task<IActionResult> Shipping(string? searchTerm)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.Title.Key];
            ViewData["SearchTerm"] = searchTerm;
            var governorates = await _mediator.Send(new GetShippingGovernoratesQuery(searchTerm, IncludeInactive: true));
            return View(governorates);
        }

        [HttpGet("shipping/create")]
        public IActionResult CreateShipping()
        {
            ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.AddNew.Key];
            return View("AddShippingGovernorate", new ShippingGovernorateFormViewModel());
        }

        [HttpPost("shipping/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipping(ShippingGovernorateFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.AddNew.Key];
                return View("AddShippingGovernorate", model);
            }

            try
            {
                await _mediator.Send(new CreateShippingGovernorateCommand(
                    model.Name, model.ArName, model.ShippingPrice, model.DisplayOrder));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.AdminShipping.CreatedSuccess.Key].ToString();
                return RedirectToAction(nameof(Shipping));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.AddNew.Key];
            return View("AddShippingGovernorate", model);
        }

        [HttpGet("shipping/edit/{id}")]
        public async Task<IActionResult> EditShipping(Guid id)
        {
            ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.Edit.Key];

            var governorates = await _mediator.Send(new GetShippingGovernoratesQuery(IncludeInactive: true));
            var gov = governorates.FirstOrDefault(x => x.Id == id);
            if (gov == null)
                return NotFound();

            var viewModel = new ShippingGovernorateFormViewModel
            {
                Id = gov.Id,
                Name = gov.Name,
                ArName = gov.ArName,
                ShippingPrice = gov.ShippingPrice,
                DisplayOrder = gov.DisplayOrder,
                IsActive = gov.IsActive
            };

            return View("AddShippingGovernorate", viewModel);
        }

        [HttpPost("shipping/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShipping(Guid id, ShippingGovernorateFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.Edit.Key];
                return View("AddShippingGovernorate", model);
            }

            try
            {
                await _mediator.Send(new UpdateShippingGovernorateCommand(
                    id, model.Name, model.ArName, model.ShippingPrice, model.DisplayOrder, model.IsActive));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.AdminShipping.UpdatedSuccess.Key].ToString();
                return RedirectToAction(nameof(Shipping));
            }
            catch (BadRequestException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            }

            ViewData["Title"] = _localizer[LocalizationKeys.AdminShipping.Edit.Key];
            return View("AddShippingGovernorate", model);
        }

        [HttpPost("shipping/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShipping(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteShippingGovernorateCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.AdminShipping.DeletedSuccess.Key].ToString();
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ShippingMessages.NotFound.Key].ToString();
            }

            return RedirectToAction(nameof(Shipping));
        }

        [HttpPost("shipping/toggle-status/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleShippingStatus(Guid id)
        {
            try
            {
                await _mediator.Send(new ToggleShippingGovernorateStatusCommand(id));
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.AdminShipping.ToggledSuccess.Key].ToString();
            }
            catch (NotFoundException)
            {
                TempData["ErrorMessage"] = _localizer[LocalizationKeys.ShippingMessages.NotFound.Key].ToString();
            }

            return RedirectToAction(nameof(Shipping));
        }
    }
}
