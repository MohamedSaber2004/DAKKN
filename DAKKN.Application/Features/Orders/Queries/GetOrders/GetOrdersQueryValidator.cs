using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
    {
        public GetOrdersQueryValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidValue.Value]);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidValue.Value]);
        }
    }
}
