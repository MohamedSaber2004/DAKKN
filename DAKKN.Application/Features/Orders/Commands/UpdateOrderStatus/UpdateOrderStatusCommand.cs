using DAKKN.Domain.Enums;
using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus NewStatus, string? Notes = null) : IRequest;
}
