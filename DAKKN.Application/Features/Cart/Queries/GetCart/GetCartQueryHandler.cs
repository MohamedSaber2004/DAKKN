using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;
using MediatR;

namespace DAKKN.Application.Features.Cart.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly IGuestCartStorage _cartStorage;

        public GetCartQueryHandler(IGuestCartStorage cartStorage)
        {
            _cartStorage = cartStorage;
        }

        public Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var items = _cartStorage.GetCart();
            return Task.FromResult(new CartDto { Items = items });
        }
    }
}
