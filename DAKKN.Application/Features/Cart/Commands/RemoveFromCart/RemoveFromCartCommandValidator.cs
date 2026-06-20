using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Cart.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
    {
        public RemoveFromCartCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key]);
        }
    }
}
