using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class CustomerOrderDetailsViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        
        // Shipping Info
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingGovernorate { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        
        // Order Items
        public List<OrderItemViewModel> Items { get; set; } = new();
        
        // Totals
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount => Subtotal + ShippingFee;
        
        // Tracking info if available
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        
        // System Logs for Customer (Simplified)
        public List<OrderLogViewModel> Logs { get; set; } = new();
    }
}
