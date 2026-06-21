using MediatR;

namespace DAKKN.Application.Features.Favorites.Commands.ToggleFavorite
{
    public record ToggleFavoriteCommand(Guid ProductId) : IRequest<bool>;
}
