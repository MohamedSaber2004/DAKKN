namespace DAKKN.Application.Features.Cart.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(x => x.Quantity);
        public decimal Subtotal => Items.Sum(x => x.Price * x.Quantity);
        public Guid? ShippingGovernorateId { get; set; }
        public string? GovernorateName { get; set; }
        public string? GovernorateArName { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total => Subtotal + ShippingPrice;
    }
}
