using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Cart.DTOs;

namespace DAKKN.MVC.ViewModels.Landing
{
    public class GuestCartViewModel
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalQuantity => Items.Sum(x => x.Quantity);
        public decimal Subtotal => Items.Sum(x => x.Price * x.Quantity);
        public Guid? ShippingGovernorateId { get; set; }
        public string? GovernorateName { get; set; }
        public string? GovernorateArName { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Total => Subtotal + ShippingPrice;
        public bool IsEmpty => Items.Count == 0;
        public List<ShippingGovernorateDto> Governorates { get; set; } = new();
    }
}
