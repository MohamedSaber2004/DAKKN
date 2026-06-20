using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Cart.Commands.UpdateCartShipping
{
    public class UpdateCartShippingCommandValidator : AbstractValidator<UpdateCartShippingCommand>
    {
        public UpdateCartShippingCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.ShippingGovernorateId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key]);
        }
    }
}
