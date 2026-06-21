using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidEnum.Value]);

            RuleFor(x => x.Notes)
                .MaximumLength(500);
        }
    }
}
