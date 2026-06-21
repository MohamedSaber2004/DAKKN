using MediatR;

namespace DAKKN.Application.Features.Favorites.Commands.RemoveFavorite
{
    public record RemoveFavoriteCommand(Guid ProductId) : IRequest;
}
