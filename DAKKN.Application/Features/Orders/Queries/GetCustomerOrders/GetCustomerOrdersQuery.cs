using DAKKN.Application.Features.Orders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.GetCustomerOrders
{
    public record GetCustomerOrdersQuery : IRequest<List<OrderListDto>>;
}
