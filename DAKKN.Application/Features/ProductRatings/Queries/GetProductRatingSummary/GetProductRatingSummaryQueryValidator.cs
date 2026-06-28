using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.ProductRatings.Queries.GetProductRatingSummary
{
    public class GetProductRatingSummaryQueryValidator : AbstractValidator<GetProductRatingSummaryQuery>
    {
        public GetProductRatingSummaryQueryValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.ProductId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
        }
    }
}
