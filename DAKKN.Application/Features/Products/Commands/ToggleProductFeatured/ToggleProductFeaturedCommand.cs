using MediatR;

namespace DAKKN.Application.Features.Products.Commands.ToggleProductFeatured
{
    public record ToggleProductFeaturedCommand(Guid ProductId) : IRequest<bool>;
}
