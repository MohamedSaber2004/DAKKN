using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Support.Commands.AddInternalNote
{
    public class AddInternalNoteCommandValidator : AbstractValidator<AddInternalNoteCommand>
    {
        public AddInternalNoteCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.TicketId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Note)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(2000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 2000]);
        }
    }
}
