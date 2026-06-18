using MediatR;

namespace DAKKN.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
}
