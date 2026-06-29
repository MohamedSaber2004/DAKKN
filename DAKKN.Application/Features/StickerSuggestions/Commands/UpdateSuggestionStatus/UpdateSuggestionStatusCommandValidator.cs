using DAKKN.Application.Localization;
using DAKKN.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.UpdateSuggestionStatus
{
    public class UpdateSuggestionStatusCommandValidator : AbstractValidator<UpdateSuggestionStatusCommand>
    {
        public UpdateSuggestionStatusCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.SuggestionId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.NewStatus)
                .IsInEnum().WithMessage(localizer[LocalizationKeys.ValidationMessages.InvalidEnum.Value]);

            RuleFor(v => v.AdminNote)
                .MaximumLength(2000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 2000])
                .When(v => v.AdminNote is not null);
        }
    }
}
