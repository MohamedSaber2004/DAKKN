using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetMostOrderedProducts
{
    public record GetMostOrderedProductsQuery(int Count = 4) : IRequest<List<ProductDto>>;
}
