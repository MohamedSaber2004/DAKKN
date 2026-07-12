using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.CustomOrders.Commands.CreateCustomOrder
{
    public class CreateCustomOrderCommandValidator : AbstractValidator<CreateCustomOrderCommand>
    {
        public CreateCustomOrderCommandValidator(IStringLocalizer<Messages> localizer)
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

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(x => x.Notes)
                .MaximumLength(1000);
        }
    }
}
