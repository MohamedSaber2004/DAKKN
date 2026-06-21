using FluentValidation;

namespace DAKKN.Application.Features.Favorites.Commands.RemoveFavorite
{
    public class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
    {
        public RemoveFavoriteCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}
