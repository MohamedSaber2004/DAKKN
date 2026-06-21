using DAKKN.Domain.Enums;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class CustomerOrderListViewModel
    {
        public List<CustomerOrderItemViewModel> Orders { get; set; } = new();
    }

    public class CustomerOrderItemViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CustomerOrderDetailsViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingGovernorate { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        public List<CustomerOrderItemDetailViewModel> Items { get; set; } = new();
        public List<CustomerOrderStatusHistoryViewModel> StatusHistory { get; set; } = new();
    }

    public class CustomerOrderItemDetailViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CustomerOrderStatusHistoryViewModel
    {
        public OrderStatus Status { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
