using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.CreateShippingGovernorate
{
    public class CreateShippingGovernorateCommandValidator : AbstractValidator<CreateShippingGovernorateCommand>
    {
        public CreateShippingGovernorateCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MaximumLength(100).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Key]);

            RuleFor(x => x.ArName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MaximumLength(100).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Key]);

            RuleFor(x => x.ShippingPrice)
                .GreaterThanOrEqualTo(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Key]);

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Key]);
        }
    }
}
