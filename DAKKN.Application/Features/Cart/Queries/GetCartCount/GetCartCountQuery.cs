using MediatR;

namespace DAKKN.Application.Features.Cart.Queries.GetCartCount
{
    public record GetCartCountQuery : IRequest<int>;
}
