using DAKKN.Application.Features.Orders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.GetRecentOrders
{
    public record GetRecentOrdersQuery(int Count = 10) : IRequest<List<RecentOrderDto>>;
}
