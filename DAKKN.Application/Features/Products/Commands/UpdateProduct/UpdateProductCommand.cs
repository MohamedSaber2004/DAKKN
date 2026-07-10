using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string ArName,
        string? Description,
        string? ArDescription,
        decimal Price,
        string ImageUrl,
        List<string> FinishOptions,
        List<string> SizeOptions,
        Guid CategoryId,
        int QuantityInStock,
        int DangerQuantity
    ) : IRequest<ProductDto>;
}
