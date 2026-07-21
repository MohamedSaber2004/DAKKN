using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.CustomOrders.DTOs
{
    public class CustomOrderListDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public CustomOrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CustomOrderDetailDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
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

    public class ExportUndeliveredCustomOrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomOrderDto
    {
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string ShippingAddress { get; set; } = null!;
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public string? Shape { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalAmount { get; set; }
    }
}
