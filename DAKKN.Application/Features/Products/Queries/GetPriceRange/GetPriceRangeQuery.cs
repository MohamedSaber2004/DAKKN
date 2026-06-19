using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetPriceRange
{
    public record GetPriceRangeQuery : IRequest<PriceRangeDto>;
}
