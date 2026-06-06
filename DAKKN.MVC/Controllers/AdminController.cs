using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.MVC.Controllers
{
    [Route("admin")]
    // [Authorize(Roles = "Admin")]   // wire up once real auth is implemented
    public class AdminController : Controller
    {
        [HttpGet("")]
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            ViewData["Title"] = "نظرة عامة";
            return View();
        }

        [HttpGet("users")]
        public IActionResult Users()
        {
            ViewData["Title"] = "المستخدمون";
            return View("Placeholder", "المستخدمون");
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            ViewData["Title"] = "الطلبات";
            
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
            ViewData["Title"] = "تفاصيل الطلب";
            
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
            ViewData["Title"] = "المخزون";
            return View("Placeholder", "المخزون");
        }

        [HttpGet("support")]
        public IActionResult Support()
        {
            ViewData["Title"] = "الدعم";
            
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

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            ViewData["Title"] = "الإعدادات";
            return View();
        }

        [HttpGet("add-product")]
        public IActionResult AddProduct()
        {
            ViewData["Title"] = "إضافة منتج";
            return View("Placeholder", "إضافة منتج جديد");
        }
    }
}
