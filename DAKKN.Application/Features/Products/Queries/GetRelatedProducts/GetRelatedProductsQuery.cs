using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetRelatedProducts
{
    public record GetRelatedProductsQuery(Guid ProductId, Guid CategoryId) : IRequest<List<ProductDto>>;
}
