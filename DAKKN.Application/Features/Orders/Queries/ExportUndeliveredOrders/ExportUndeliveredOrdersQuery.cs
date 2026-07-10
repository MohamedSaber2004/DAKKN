using DAKKN.Application.Features.Orders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.ExportUndeliveredOrders
{
    public record ExportUndeliveredOrdersQuery : IRequest<List<ExportUndeliveredOrderDto>>;
}
