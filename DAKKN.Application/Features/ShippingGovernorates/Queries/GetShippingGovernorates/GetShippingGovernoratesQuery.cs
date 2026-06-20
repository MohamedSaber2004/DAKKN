using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Queries.GetShippingGovernorates
{
    public record GetShippingGovernoratesQuery(string? SearchTerm = null, bool IncludeInactive = false) : IRequest<List<ShippingGovernorateDto>>;
}
