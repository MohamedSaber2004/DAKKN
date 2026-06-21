using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(200);

            RuleFor(x => x.CustomerPhone)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(50);

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(500);

            RuleFor(x => x.ShippingGovernorateId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }
    }
}
