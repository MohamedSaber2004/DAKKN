using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string ArName,
        string Description,
        string ArDescription,
        decimal Price,
        string ImageUrl,
        List<string> FinishOptions,
        List<string> SizeOptions,
        Guid CategoryId
    ) : IRequest<ProductDto>;
}
