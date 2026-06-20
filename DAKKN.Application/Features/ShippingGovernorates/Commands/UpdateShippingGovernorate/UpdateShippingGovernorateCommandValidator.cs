using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.UpdateShippingGovernorate
{
    public class UpdateShippingGovernorateCommandValidator : AbstractValidator<UpdateShippingGovernorateCommand>
    {
        public UpdateShippingGovernorateCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key]);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MaximumLength(100).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Key]);

            RuleFor(x => x.ArName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MaximumLength(100).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Key]);

            RuleFor(x => x.ShippingPrice)
                .GreaterThanOrEqualTo(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Key]);
        }
    }
}
