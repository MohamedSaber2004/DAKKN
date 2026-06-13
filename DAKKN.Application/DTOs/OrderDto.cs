using System;

namespace DAKKN.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // e.g., "Processing", "Shipped", "Delivered"
        public int ItemCount { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
