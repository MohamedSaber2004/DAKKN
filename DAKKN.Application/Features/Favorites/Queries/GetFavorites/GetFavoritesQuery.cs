using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Favorites.Queries.GetFavorites
{
    public record GetFavoritesQuery : IRequest<List<ProductDto>>;
}
