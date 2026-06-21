namespace DAKKN.MVC.ViewModels.Customer
{
    public class PlaceOrderRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public Guid ShippingGovernorateId { get; set; }
        public string? Notes { get; set; }
    }
}
