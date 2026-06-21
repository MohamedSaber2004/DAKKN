using DAKKN.Application.Features.Orders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.GetOrderDetails
{
    public record GetOrderDetailsQuery(Guid OrderId, bool IsAdmin = false) : IRequest<OrderDetailDto>;
}
