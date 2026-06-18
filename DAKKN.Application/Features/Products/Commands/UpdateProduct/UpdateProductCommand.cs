using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string ImageUrl,
        List<string> FinishOptions,
        List<string> SizeOptions,
        Guid CategoryId
    ) : IRequest<ProductDto>;
}
