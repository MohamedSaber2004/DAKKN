using DAKKN.Application.Interfaces;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity
{
    public class UpdateCartQuantityCommandHandler : IRequestHandler<UpdateCartQuantityCommand, int>
    {
        private readonly IGuestCartStorage _cartStorage;

        public UpdateCartQuantityCommandHandler(IGuestCartStorage cartStorage)
        {
            _cartStorage = cartStorage;
        }

        public Task<int> Handle(UpdateCartQuantityCommand request, CancellationToken cancellationToken)
        {
            var cart = _cartStorage.GetCart();
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
            if (existing != null)
            {
                existing.Quantity = Math.Max(1, request.Quantity);
            }
            _cartStorage.SetCart(cart);
            return Task.FromResult(_cartStorage.GetCartCount());
        }
    }
}
