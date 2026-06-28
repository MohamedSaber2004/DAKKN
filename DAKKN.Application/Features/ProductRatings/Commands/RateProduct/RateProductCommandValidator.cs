using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ProductRatings.Commands.RateProduct
{
    public class RateProductCommandValidator : AbstractValidator<RateProductCommand>
    {
        public RateProductCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.ProductId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Stars)
                .InclusiveBetween(1, 5).WithMessage(localizer[LocalizationKeys.ValidationMessages.Range.Value, 1, 5]);
        }
    }
}
