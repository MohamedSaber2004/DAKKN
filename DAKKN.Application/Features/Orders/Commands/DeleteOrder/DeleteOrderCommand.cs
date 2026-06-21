using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.DeleteOrder
{
    public record DeleteOrderCommand(Guid OrderId) : IRequest;
}
