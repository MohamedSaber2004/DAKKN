using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(x => x.Reason)
                .MaximumLength(500);
        }
    }
}
