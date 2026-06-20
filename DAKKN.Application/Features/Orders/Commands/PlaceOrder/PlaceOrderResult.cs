using System;
using System.Collections.Generic;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderResult
    {
        public string OrderNumber { get; set; } = string.Empty;
        public List<OrderItemResult> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class OrderItemResult
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
