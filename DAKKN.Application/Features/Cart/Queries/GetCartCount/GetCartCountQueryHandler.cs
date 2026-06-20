using DAKKN.Application.Interfaces;
using MediatR;

namespace DAKKN.Application.Features.Cart.Queries.GetCartCount
{
    public class GetCartCountQueryHandler : IRequestHandler<GetCartCountQuery, int>
    {
        private readonly IGuestCartStorage _cartStorage;

        public GetCartCountQueryHandler(IGuestCartStorage cartStorage)
        {
            _cartStorage = cartStorage;
        }

        public Task<int> Handle(GetCartCountQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_cartStorage.GetCartCount());
        }
    }
}
