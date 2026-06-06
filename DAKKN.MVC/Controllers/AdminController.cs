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
            return View("Placeholder", "الطلبات");
        }

        [HttpGet("order-details/{id}")]
        public IActionResult OrderDetails(int id)
        {
            ViewData["Title"] = "تفاصيل الطلب";
            return View("Placeholder", $"تفاصيل الطلب #{id}");
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
