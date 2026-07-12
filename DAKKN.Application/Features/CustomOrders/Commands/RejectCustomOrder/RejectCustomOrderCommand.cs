using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Commands.RejectCustomOrder
{
    public record RejectCustomOrderCommand(Guid Id) : IRequest<bool>;
}
