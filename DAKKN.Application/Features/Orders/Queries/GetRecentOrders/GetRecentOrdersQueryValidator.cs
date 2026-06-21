using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Queries.GetRecentOrders
{
    public class GetRecentOrdersQueryValidator : AbstractValidator<GetRecentOrdersQuery>
    {
        public GetRecentOrdersQueryValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.Count)
                .InclusiveBetween(1, 50).WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidValue.Value]);
        }
    }
}
