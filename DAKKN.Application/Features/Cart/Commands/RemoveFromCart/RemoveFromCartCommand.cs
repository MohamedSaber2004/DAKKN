using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.RemoveFromCart
{
    public record RemoveFromCartCommand(Guid ProductId) : IRequest<int>;
}
