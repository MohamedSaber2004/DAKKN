using DAKKN.Domain.Enums;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class AdminCustomOrderListViewModel
    {
        public List<AdminCustomOrderItemViewModel> Orders { get; set; } = new();
        public int TotalCount { get; set; }
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public CustomOrderStatus? FilterStatus { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 20;
    }

    public class AdminCustomOrderItemViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public CustomOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminCustomOrderDetailsViewModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public string? Shape { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public CustomOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
