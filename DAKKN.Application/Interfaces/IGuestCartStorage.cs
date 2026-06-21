using DAKKN.Application.Features.Cart.DTOs;

namespace DAKKN.Application.Interfaces
{
    public interface IGuestCartStorage
    {
        List<CartItemDto> GetCart();
        void SetCart(List<CartItemDto> items);
        int GetCartCount();
        void ClearCart();
        Guid? GetShippingGovernorateId();
        void SetShippingGovernorateId(Guid? governorateId);
    }
}
