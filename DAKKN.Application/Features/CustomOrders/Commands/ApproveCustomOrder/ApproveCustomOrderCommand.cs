using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Commands.ApproveCustomOrder
{
    public record ApproveCustomOrderCommand(Guid Id) : IRequest<bool>;
}
