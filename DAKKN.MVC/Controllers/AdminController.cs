using DAKKN.Appearence.Filters;
using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
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
using DAKKN.Application.Features.Users.Commands.UpdateUserSettings;
using DAKKN.Application.Features.Users.Queries.ExportUsers;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using DAKKN.Application.Features.Users.Queries.GetUserSettings;
using DAKKN.Application.Features.Users.Queries.GetUserStats;
using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using DAKKN.MVC.ViewModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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

        public AdminController(IWebHostEnvironment env,IStringLocalizer<Messages> localizer, IMediator mediator, IImageValidator imageValidator)
        {
            _env = env;
            _localizer = localizer;
            _mediator = mediator;
            _imageValidator = imageValidator;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = _localizer["admin_overview"];
            return View();
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
                    ? $"/files/{u.ProfilePictureUrl}"
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

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = _localizer["admin_orders"];
            
            var mockOrders = new List<OrderListItemViewModel>
            {
                new OrderListItemViewModel { OrderId = "ORD-001", CustomerName = "أحمد محمود", OrderDate = DateTime.Now.AddDays(-1), Status = OrderStatus.Processing, TotalAmount = 750 },
                new OrderListItemViewModel { OrderId = "ORD-002", CustomerName = "سارة عبد الله", OrderDate = DateTime.Now.AddDays(-2), Status = OrderStatus.Shipped, TotalAmount = 1200 },
                new OrderListItemViewModel { OrderId = "ORD-003", CustomerName = "ياسين علي", OrderDate = DateTime.Now.AddDays(-3), Status = OrderStatus.Delivered, TotalAmount = 450 },
                new OrderListItemViewModel { OrderId = "ORD-004", CustomerName = "ليلى محمد", OrderDate = DateTime.Now.AddDays(-5), Status = OrderStatus.Cancelled, TotalAmount = 300 }
            };

            var viewModel = new OrderListViewModel
            {
                Orders = mockOrders,
                TotalOrders = 2458,
                ProcessingCount = 142,
                ShippedCount = 856,
                MonthlyRevenue = "124,000 ج.م"
            };

            return View(viewModel);
        }

        [HttpGet("order-details/{id}")]
        public IActionResult OrderDetails(string id)
        {
            ViewData["Title"] = _localizer["admin_order_details"];
            
            var viewModel = new OrderDetailsViewModel
            {
                OrderId = id,
                Status = OrderStatus.TechnicalReview,
                OrderDate = DateTime.Now.AddDays(-2),
                DeadlineDate = DateTime.Now.AddDays(2),
                Material = "فينيل ممتاز (Vinyl)",
                Finish = "هولوجرام",
                CutType = "قص محيطي (Die-Cut)",
                Resolution = "300 DPI",
                InternalNotes = "العميل طلب ألوان مشبعة أكثر في التصميم الثاني.",
                Items = new List<OrderItemViewModel>
                {
                    new OrderItemViewModel { 
                        ProductName = "حزمة ملصقات جيمنج", 
                        Sku = "GM-8821", 
                        Dimensions = "5x5 سم", 
                        Quantity = 50, 
                        UnitPrice = 15, 
                        FileStatusOk = true,
                        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBdJqwUIJElF6yKiAthjzqbJT3oybHZLiBv1kJqmbuD5er1OlaDE6e1hoP4RbXI68dtTm9QrVrdOu4_L3gIbRZzTfdPkJLIImwJgtfFOblkSv-zMbSFfD_U6GblKhzSYn6aZ6UMAQTaHHAv7ja5lVj5JQ_0eA60WnQU6Tdn8_-P_aPc-MlDlgr3RCXgvrDq7VwR6wWvaxjG5_se3JpJkYk1JpuDc-70Yt9bnuSg2R_zVgHYwNGlkw_-8bAWpGARNWW3NW7GDdgZKL8"
                    },
                    new OrderItemViewModel { 
                        ProductName = "تصميم خاص (تايبوجرافي)", 
                        Sku = "رفع بواسطة العميل", 
                        Dimensions = "10x4 سم", 
                        Quantity = 100, 
                        UnitPrice = 8.5m, 
                        FileStatusOk = false,
                        ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAYqCGBR3sHhdqe1mdJisODxn5Q3mYWRwLi66x75IT4A9Yxij9GP1QnCZjiAUbcOSFgViSTcKrRe02xppzY4S8p2-kPQOJMDgqlv2RT4QI2eFpHWZBASZagZoaX61ZEZKFfuufofS4xvl9pMujFf1iGLw_v3AvaaNeVwVJhlVSUwYUZoFpvIHFOFBAgAYLevbS7VwjTYNJ_NDmU3Mf3ggAVKxNHE7vl2mY5kv_zNm-ME8QUmPaEUO243hcgumpk5LD8o9Gl1101g10"
                    }
                },
                Customer = new CustomerInfoViewModel
                {
                    Name = "أحمد محمود",
                    Email = "ahmed.m@example.com",
                    TotalSpent = "4,500 ج.م",
                    TotalOrdersCount = 12,
                    RecentOrders = new List<CustomerRecentOrderViewModel>
                    {
                        new CustomerRecentOrderViewModel { OrderId = "98201", Status = OrderStatus.Delivered },
                        new CustomerRecentOrderViewModel { OrderId = "97844", Status = OrderStatus.Delivered },
                        new CustomerRecentOrderViewModel { OrderId = "97102", Status = OrderStatus.Cancelled }
                    }
                },
                Logs = new List<OrderLogViewModel>
                {
                    new OrderLogViewModel { Message = "تم تأكيد الدفع بنجاح (Paymob).", Timestamp = "منذ ساعتين", Actor = "النظام", IsSystem = true },
                    new OrderLogViewModel { Message = "تم رفع ملفات التصميم الجديدة.", Timestamp = "منذ 3 ساعات", Actor = "أحمد (العميل)", IsSystem = false },
                    new OrderLogViewModel { Message = "تم إنشاء الطلب.", Timestamp = "أمس", Actor = "النظام", IsSystem = true }
                }
            };

            return View(viewModel);
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
            ViewData["Title"] = _localizer["admin_support"];
            
            var mockTickets = new List<SupportTicketViewModel>
            {
                new SupportTicketViewModel { 
                    TicketId = "TK-1001", 
                    CustomerName = "أحمد محمد", 
                    CustomerEmail = "ahmed@example.com", 
                    Subject = "تأخر في وصول الطلب", 
                    MessageSnippet = "لقد طلبت ملصقات منذ 5 أيام ولم تصل حتى الآن...", 
                    CreatedAt = DateTime.Now.AddHours(-2), 
                    Status = TicketStatus.Open, 
                    Priority = TicketPriority.High 
                },
                new SupportTicketViewModel { 
                    TicketId = "TK-1002", 
                    CustomerName = "سارة أحمد", 
                    CustomerEmail = "sara@example.com", 
                    Subject = "خطأ في تصميم الملصق", 
                    MessageSnippet = "الألوان في الملصق الذي وصلني مختلفة عن التصميم الأصلي...", 
                    CreatedAt = DateTime.Now.AddDays(-1), 
                    Status = TicketStatus.InProgress, 
                    Priority = TicketPriority.Medium 
                },
                new SupportTicketViewModel { 
                    TicketId = "TK-1003", 
                    CustomerName = "ياسين علي", 
                    CustomerEmail = "yassin@example.com", 
                    Subject = "طلب جملة مخصص", 
                    MessageSnippet = "أريد طلب 500 ملصق لشعار شركتي، هل يوجد خصم؟", 
                    CreatedAt = DateTime.Now.AddDays(-2), 
                    Status = TicketStatus.Resolved, 
                    Priority = TicketPriority.Low 
                }
            };

            var viewModel = new SupportDashboardViewModel
            {
                Tickets = mockTickets,
                TotalOpen = 12,
                WaitingResponse = 5,
                AverageResolutionTime = "18h"
            };

            return View(viewModel);
        }

        [HttpGet("content")]
        public IActionResult Content()
        {
            ViewData["Title"] = _localizer["admin_content_mgmt"];
            var model = new ContentManagementViewModel();
            return View(model);
        }

        [HttpPost("content/update")]
        public IActionResult UpdateContent([FromBody] ContentManagementViewModel model)
        {
            // Logic to save settings to database would go here
            return Json(new { success = true });
        }

        [HttpGet("settings")]
        public async Task<IActionResult> Settings()
        {
            ViewData["Title"] = _localizer["admin_settings"];

            var settings = await _mediator.Send(new GetUserSettingsQuery());

            var viewModel = new AdminSettingsViewModel
            {
                FullName         = settings.FullName,
                Email            = settings.Email,
                Language         = settings.Language,
                Theme            = settings.Theme,
                PrimaryColor     = settings.PrimaryColor,
                IsDarkMode       = settings.IsDarkMode,
                LayoutMode       = settings.LayoutMode,
                ProfilePictureUrl = settings.ProfilePictureUrl
            };

            return View(viewModel);
        }

        [HttpPost("settings")]
        public async Task<IActionResult> Settings(AdminSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_settings"];
                return View(model);
            }

            // Get current settings to check if language changed
            var currentSettings = await _mediator.Send(new GetUserSettingsQuery());

            await _mediator.Send(new UpdateUserSettingsCommand(
                model.FullName,
                model.Language,
                model.Theme,
                model.PrimaryColor,
                model.IsDarkMode,
                model.LayoutMode
            ));

            TempData["SuccessMessage"] = "profile_updated_success";

            // If language changed, redirect through SetLanguage to update cookie
            if (currentSettings.Language != model.Language)
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
                null, null, null, null, null, null,
                ProfileImage: profileImage));

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
                string? imageUrl = string.IsNullOrEmpty(model.ExistingImageUrl) ? null : model.ExistingImageUrl;

                if (model.ImageFile != null)
                {
                    var oldFileName = string.IsNullOrEmpty(model.ExistingImageUrl)
                        ? null
                        : Path.GetFileName(model.ExistingImageUrl.TrimEnd('/'));

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
            var viewModel = new AddProductViewModel
            {
                AvailableCategories = categories
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

            var imageUrl = string.IsNullOrEmpty(model.ExistingImageUrl) ? string.Empty : model.ExistingImageUrl;

            if (model.ImageFile != null)
            {
                var result = await _mediator.Send(new UpdateImageCommand
                {
                    File = model.ImageFile,
                    UploadPlace = 1
                });
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                imageUrl = $"{baseUrl}/files/{result}";
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

            await _mediator.Send(command);

            TempData["SuccessMessage"] = _localizer[LocalizationKeys.Products.Created.Value].ToString();
            return RedirectToAction(nameof(Inventory));
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
                    ExistingImageUrl = string.IsNullOrEmpty(product.ImageUrl) || product.ImageUrl.StartsWith("http")
                        ? product.ImageUrl
                        : $"{Request.Scheme}://{Request.Host}/files/{product.ImageUrl.TrimStart('/')}",
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

            var imageUrl = string.IsNullOrEmpty(model.ExistingImageUrl) ? string.Empty : model.ExistingImageUrl;

            if (model.ImageFile != null)
            {
                var oldFileName = string.IsNullOrEmpty(model.ExistingImageUrl)
                    ? null
                    : Path.GetFileName(model.ExistingImageUrl.TrimEnd('/'));

                var result = await _mediator.Send(new UpdateImageCommand
                {
                    File = model.ImageFile,
                    UploadPlace = 1,
                    ImageName = oldFileName
                });
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                imageUrl = $"{baseUrl}/files/{result}";
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

            await _mediator.Send(command);

            TempData["SuccessMessage"] = _localizer[LocalizationKeys.Products.Updated.Value].ToString();
            return RedirectToAction(nameof(Inventory));
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
