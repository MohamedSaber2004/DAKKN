using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.Orders.DTOs
{
    public class OrderListDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Subtotal { get; set; }
        public int ItemCount { get; set; }
    }

    public class OrderDetailDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string ShippingGovernorateName { get; set; } = null!;
        public decimal ShippingCost { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderStatusHistoryDto
    {
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string ChangedBy { get; set; } = null!;
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class ExportUndeliveredOrderDto
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string OrderNumber { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
