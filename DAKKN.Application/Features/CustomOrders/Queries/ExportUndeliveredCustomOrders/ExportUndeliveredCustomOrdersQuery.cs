using DAKKN.Application.Features.CustomOrders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.CustomOrders.Queries.ExportUndeliveredCustomOrders
{
    public record ExportUndeliveredCustomOrdersQuery : IRequest<List<ExportUndeliveredCustomOrderDto>>;
}
