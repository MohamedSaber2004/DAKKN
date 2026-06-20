using MediatR;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public record PlaceOrderCommand : IRequest<PlaceOrderResult>;
}
