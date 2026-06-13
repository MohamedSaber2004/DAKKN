using System;
using System.Collections.Generic;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class OrderListItemViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        TechnicalReview
    }

    public class OrderListViewModel
    {
        public List<OrderListItemViewModel> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public int ProcessingCount { get; set; }
        public int ShippedCount { get; set; }
        public string MonthlyRevenue { get; set; } = "0 ج.م";
    }

    public class OrderDetailsViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        
        // Technical Specs
        public string Material { get; set; } = string.Empty;
        public string Finish { get; set; } = string.Empty;
        public string CutType { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;

        public List<OrderItemViewModel> Items { get; set; } = new();
        
        // Customer Info
        public CustomerInfoViewModel Customer { get; set; } = new();
        
        // Logs
        public List<OrderLogViewModel> Logs { get; set; } = new();
        
        public string InternalNotes { get; set; } = string.Empty;
    }

    public class OrderItemViewModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Dimensions { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public bool FileStatusOk { get; set; }
    }

    public class CustomerInfoViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TotalSpent { get; set; } = string.Empty;
        public int TotalOrdersCount { get; set; }
        public List<CustomerRecentOrderViewModel> RecentOrders { get; set; } = new();
    }

    public class CustomerRecentOrderViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
    }

    public class OrderLogViewModel
    {
        public string Message { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public string Actor { get; set; } = string.Empty;
        public bool IsSystem { get; set; }
    }
}
