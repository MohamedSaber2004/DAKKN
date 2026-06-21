using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Queries.GetOrderDetails
{
    public class GetOrderDetailsQueryValidator : AbstractValidator<GetOrderDetailsQuery>
    {
        public GetOrderDetailsQueryValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
        }
    }
}
