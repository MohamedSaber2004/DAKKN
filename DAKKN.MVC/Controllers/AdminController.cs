using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using DAKKN.Application.Localization;
using DAKKN.MVC.ViewModels.Admin;

using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using System.Security.Claims;
using MediatR;
using DAKKN.Application.Features.Users.Queries.GetUserStats;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DAKKN.MVC.Controllers
{
    [Route("admin")]
    [RoleAuthorize(UserType.Admin)]
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IMediator _mediator;

        public AdminController(IWebHostEnvironment env,IStringLocalizer<Messages> localizer, IMediator mediator)
        {
            _env = env;
            _localizer = localizer;
            _mediator = mediator;
        }

        [HttpGet("")]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = _localizer["admin_overview"];
            return View();
        }

        [HttpGet("users")]
        public async Task<IActionResult> Users(string? fullName, string? email, string? phoneNumber, string? role, string? status, int pageNumber = 1, int pageSize = 20)
        {
            ViewData["Title"] = _localizer["admin_users"];

            var query = new GetAllUsersQuery(fullName, email, phoneNumber, role, status, pageNumber, pageSize);
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
                Status = u.Status == "Active" ? UserStatus.Active : UserStatus.Deleted
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

            return View(viewModel);
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
        public IActionResult Inventory()
        {
            ViewData["Title"] = _localizer["admin_inventory"];

            var mockProducts = new List<InventoryProductViewModel>
            {
                new InventoryProductViewModel { 
                    Id = "PRD-001", Name = "كاسيت ريترو", Category = "Anime", Price = 70, StockLevel = 150, Sku = "STK-RETR-01",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBdJqwUIJElF6yKiAthjzqbJT3oybHZLiBv1kJqmbuD5er1OlaDE6e1hoP4RbXI68dtTm9QrVrdOu4_L3gIbRZzTfdPkJLIImwJgtfFOblkSv-zMbSFfD_U6GblKhzSYn6aZ6UMAQTaHHAv7ja5lVj5JQ_0eA60WnQU6Tdn8_-P_aPc-MlDlgr3RCXgvrDq7VwR6wWvaxjG5_se3JpJkYk1JpuDc-70Yt9bnuSg2R_zVgHYwNGlkw_-8bAWpGARNWW3NW7GDdgZKL8"
                },
                new InventoryProductViewModel { 
                    Id = "PRD-002", Name = "جمجمة نيون", Category = "Gaming", Price = 80, StockLevel = 5, Sku = "STK-NEON-02",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuAYqCGBR3sHhdqe1mdJisODxn5Q3mYWRwLi66x75IT4A9Yxij9GP1QnCZjiAUbcOSFgViSTcKrRe02xppzY4S8p2-kPQOJMDgqlv2RT4QI2eFpHWZBASZagZoaX61ZEZKFfuufofS4xvl9pMujFf1iGLw_v3AvaaNeVwVJhlVSUwYUZoFpvIHFOFBAgAYLevbS7VwjTYNJ_NDmU3Mf3ggAVKxNHE7vl2mY5kv_zNm-ME8QUmPaEUO243hcgumpk5LD8o9Gl1101g10"
                },
                new InventoryProductViewModel { 
                    Id = "PRD-003", Name = "قلب بكسل", Category = "Gaming", Price = 60, StockLevel = 85, Sku = "STK-PIXL-03",
                    ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBdJqwUIJElF6yKiAthjzqbJT3oybHZLiBv1kJqmbuD5er1OlaDE6e1hoP4RbXI68dtTm9QrVrdOu4_L3gIbRZzTfdPkJLIImwJgtfFOblkSv-zMbSFfD_U6GblKhzSYn6aZ6UMAQTaHHAv7ja5lVj5JQ_0eA60WnQU6Tdn8_-P_aPc-MlDlgr3RCXgvrDq7VwR6wWvaxjG5_se3JpJkYk1JpuDc-70Yt9bnuSg2R_zVgHYwNGlkw_-8bAWpGARNWW3NW7GDdgZKL8"
                }
            };

            var viewModel = new InventoryViewModel
            {
                Products = mockProducts,
                TotalProducts = 1248,
                LowStockAlerts = 23,
                CategoriesCount = 14,
                TotalStockValue = "85,400 ج.م"
            };

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
        public IActionResult Settings()
        {
            ViewData["Title"] = _localizer["admin_settings"];
            
            var fullName = User.FindFirstValue("FullName");
            if (string.IsNullOrWhiteSpace(fullName))
            {
                fullName = User.Identity?.Name ?? "Admin";
            }
            
            var nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            var viewModel = new AdminSettingsViewModel
            {
                FirstName = nameParts.FirstOrDefault() ?? "Admin",
                LastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "",
                Email = User.Identity?.Name ?? "admin@dakkn.com"
            };

            return View(viewModel);
        }

        [HttpPost("settings")]
        public IActionResult Settings(AdminSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_settings"];
                return View(model);
            }

            // Logic to update user profile would go here
            // e.g., await _mediator.Send(new UpdateUserProfileCommand { ... });

            TempData["SuccessMessage"] = "profile_updated_success";
            return RedirectToAction(nameof(Settings));
        }

        [HttpGet("add-product")]
        public IActionResult AddProduct()
        {
            ViewData["Title"] = _localizer["admin_add_product"];
            var viewModel = new AddProductViewModel();
            return View(viewModel);
        }

        [HttpGet("edit-product/{id}")]
        public IActionResult EditProduct(string id)
        {
            ViewData["Title"] = _localizer["admin_product_edit_title"];

            // Fetch mock product details
            var viewModel = new AddProductViewModel
            {
                Id = id,
                Name = "كاسيت ريترو",
                Description = "ملصق فينيل عالي الجودة بتصميم كاسيت كلاسيكي.",
                Category = "Anime",
                Price = 70,
                Sku = "STK-RETR-01",
                Quantity = 150,
                TrackInventory = true,
                Tags = "ريترو, كلاسيك, أنمي",
                ExistingImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBdJqwUIJElF6yKiAthjzqbJT3oybHZLiBv1kJqmbuD5er1OlaDE6e1hoP4RbXI68dtTm9QrVrdOu4_L3gIbRZzTfdPkJLIImwJgtfFOblkSv-zMbSFfD_U6GblKhzSYn6aZ6UMAQTaHHAv7ja5lVj5JQ_0eA60WnQU6Tdn8_-P_aPc-MlDlgr3RCXgvrDq7VwR6wWvaxjG5_se3JpJkYk1JpuDc-70Yt9bnuSg2R_zVgHYwNGlkw_-8bAWpGARNWW3NW7GDdgZKL8"
            };

            return View("AddProduct", viewModel);
        }

        [HttpPost("edit-product/{id}")]
        public IActionResult EditProduct(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = _localizer["admin_product_edit_title"];
                return View("AddProduct", model);
            }

            // Logic to overwrite current product details would go here
            return RedirectToAction("Inventory");
        }
    }
}
