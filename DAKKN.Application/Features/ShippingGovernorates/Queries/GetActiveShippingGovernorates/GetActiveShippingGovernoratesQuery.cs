using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates
{
    public record GetActiveShippingGovernoratesQuery : IRequest<List<ShippingGovernorateDto>>;
}
