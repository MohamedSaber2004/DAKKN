using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Cart.Commands.AddToCart
{
    public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
    {
        public AddToCartCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key]);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage(localizer[LocalizationKeys.CartMessages.QuantityMustBePositive.Key]);
        }
    }
}
