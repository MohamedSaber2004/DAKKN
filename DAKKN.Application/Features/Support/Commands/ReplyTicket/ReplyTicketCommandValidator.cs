using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Support.Commands.ReplyTicket
{
    public class ReplyTicketCommandValidator : AbstractValidator<ReplyTicketCommand>
    {
        public ReplyTicketCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.TicketId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Message)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(5000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 5000]);
        }
    }
}
