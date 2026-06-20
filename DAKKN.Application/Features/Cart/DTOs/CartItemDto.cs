namespace DAKKN.Application.Features.Cart.DTOs
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
