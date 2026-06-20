using MediatR;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartQuantity
{
    public record UpdateCartQuantityCommand(Guid ProductId, int Quantity) : IRequest<int>;
}
