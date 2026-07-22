using DAKKN.Domain.Enums;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class OrderListItemViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Subtotal { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class AdminOrderListViewModel
    {
        public List<OrderListItemViewModel> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public int PendingCount { get; set; }
        public int ProcessingCount { get; set; }
        public int ShippedCount { get; set; }
        public int DeliveredCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyDeliveryRevenue { get; set; }
        public string? SearchTerm { get; set; }
        public OrderStatus? FilterStatus { get; set; }
    }

    public class AdminOrderDetailsViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingGovernorateName { get; set; } = string.Empty;
        public decimal ShippingCost { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new();
        public List<OrderStatusHistoryViewModel> StatusHistory { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderStatusHistoryViewModel
    {
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class DashboardStatsViewModel
    {
        public int OrdersToday { get; set; }
        public int OrdersLast24Hours { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueLast24Hours { get; set; }
        public decimal DeliveryRevenueToday { get; set; }
        public decimal DeliveryRevenueLast24Hours { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
    }

    public class RecentOrderWidgetViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
