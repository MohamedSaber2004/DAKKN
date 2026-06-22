using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.BrandReviews.Commands.UpdateBrandReview
{
    public class UpdateBrandReviewCommandValidator : AbstractValidator<UpdateBrandReviewCommand>
    {
        public UpdateBrandReviewCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.Rating)
                .InclusiveBetween(1, 5).WithMessage(localizer[LocalizationKeys.ValidationMessages.Range.Value, 1, 5]);

            RuleFor(v => v.ReviewTitle)
                .MaximumLength(200).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 200]);

            RuleFor(v => v.ReviewText)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MinimumLength(3).WithMessage(localizer[LocalizationKeys.ValidationMessages.MinLength.Value, 3])
                .MaximumLength(1000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 1000]);
        }
    }
}
