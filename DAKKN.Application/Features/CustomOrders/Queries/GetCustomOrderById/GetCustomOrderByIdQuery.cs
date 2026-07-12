using DAKKN.Application.Features.CustomOrders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Queries.GetCustomOrderById
{
    public record GetCustomOrderByIdQuery(Guid Id) : IRequest<CustomOrderDetailDto?>;
}
