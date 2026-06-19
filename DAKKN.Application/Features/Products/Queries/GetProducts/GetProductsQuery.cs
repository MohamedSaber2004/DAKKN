using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Queries.GetProducts
{
    public record GetProductsQuery(
        string? SearchTerm,
        Guid? CategoryId,
        int PageNumber = PagginatedResult<ProductDto>.DefaultPageNumber,
        int PageSize = PagginatedResult<ProductDto>.DefaultPageSize,
        decimal? MaxPrice = null
    ) : IRequest<PagginatedResult<ProductDto>>;
}
