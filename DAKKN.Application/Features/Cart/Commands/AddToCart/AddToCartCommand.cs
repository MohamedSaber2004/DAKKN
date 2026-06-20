using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.AddToCart
{
    public record AddToCartCommand(Guid ProductId, int Quantity = 1) : IRequest<int>;
}
