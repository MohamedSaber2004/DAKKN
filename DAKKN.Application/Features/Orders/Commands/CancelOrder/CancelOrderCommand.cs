using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId, string? Reason = null) : IRequest;
}
