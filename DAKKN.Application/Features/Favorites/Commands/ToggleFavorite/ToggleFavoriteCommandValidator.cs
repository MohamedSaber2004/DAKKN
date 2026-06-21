using FluentValidation;

namespace DAKKN.Application.Features.Favorites.Commands.ToggleFavorite
{
    public class ToggleFavoriteCommandValidator : AbstractValidator<ToggleFavoriteCommand>
    {
        public ToggleFavoriteCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}
