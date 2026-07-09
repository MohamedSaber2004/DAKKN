using DAKKN.MVC.ViewModels.Admin;
using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class CustomerSupportDashboardViewModel
    {
        public List<SupportTicketViewModel> Tickets { get; set; } = new();
        public ContactInfoViewModel ContactInfo { get; set; } = new();
    }

    public class ContactInfoViewModel
    {
        public string Phone { get; set; } = "+20 123 456 7890";
        public string WhatsApp { get; set; } = "+201001234567";
        public string Instagram { get; set; } = "dakkn_stickers";
    }

    public class NewTicketViewModel
    {
        [Required(ErrorMessage = "validation.required")]
        [StringLength(100, MinimumLength = 5)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "validation.required")]
        public string Category { get; set; } = string.Empty;

        public string? OrderId { get; set; }

        [Required(ErrorMessage = "validation.required")]
        [StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new() { "Order Issue", "Technical Support", "Shipping", "General Inquiry" };
    }
}
