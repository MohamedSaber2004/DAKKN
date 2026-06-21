using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
        }
    }
}
