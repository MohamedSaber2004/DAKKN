using DAKKN.Application.Interfaces;
using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, int>
    {
        private readonly IGuestCartStorage _cartStorage;

        public RemoveFromCartCommandHandler(IGuestCartStorage cartStorage)
        {
            _cartStorage = cartStorage;
        }

        public Task<int> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = _cartStorage.GetCart();
            cart.RemoveAll(x => x.ProductId == request.ProductId);
            _cartStorage.SetCart(cart);
            return Task.FromResult(_cartStorage.GetCartCount());
        }
    }
}
