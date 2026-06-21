using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public record PlaceOrderCommand(
        string CustomerName,
        string CustomerPhone,
        string ShippingAddress,
        Guid ShippingGovernorateId,
        string? Notes
    ) : IRequest<PlaceOrderResult>;
}
