namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderResult
    {
        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string TrackingNumber { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public int ItemCount { get; set; }
    }
}
