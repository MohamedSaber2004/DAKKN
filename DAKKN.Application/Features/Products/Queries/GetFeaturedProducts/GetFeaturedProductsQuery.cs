using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetFeaturedProducts
{
    public record GetFeaturedProductsQuery : IRequest<List<ProductDto>>;
}
