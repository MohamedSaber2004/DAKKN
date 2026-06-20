namespace DAKKN.Application.Features.Cart.DTOs
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(x => x.Quantity);
        public decimal Subtotal => Items.Sum(x => x.Price * x.Quantity);
        public decimal Shipping => Subtotal >= 500 ? 0 : 45;
        public decimal Total => Subtotal + Shipping;
    }
}
