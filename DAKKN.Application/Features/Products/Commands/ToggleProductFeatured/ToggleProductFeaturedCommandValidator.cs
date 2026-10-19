using FluentValidation;

namespace DAKKN.Application.Features.Products.Commands.ToggleProductFeatured
{
    public class ToggleProductFeaturedCommandValidator : AbstractValidator<ToggleProductFeaturedCommand>
    {
        public ToggleProductFeaturedCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}
