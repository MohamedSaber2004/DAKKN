using DAKKN.Application.Features.Cart.DTOs;

namespace DAKKN.MVC.ViewModels.Landing
{
    public class GuestCartViewModel
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(x => x.Quantity);
        public decimal Subtotal => Items.Sum(x => x.Price * x.Quantity);
        public decimal Shipping => Subtotal >= 500 ? 0 : 45;
        public decimal Total => Subtotal + Shipping;
        public bool IsEmpty => Items.Count == 0;
        public bool IsFreeShipping => Subtotal >= 500;
        public decimal AmountLeftForFreeShipping => Subtotal >= 500 ? 0 : 500 - Subtotal;
    }
}
