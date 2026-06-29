using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.StickerSuggestions.Commands.SubmitSuggestion
{
    public class SubmitSuggestionCommandValidator : AbstractValidator<SubmitSuggestionCommand>
    {
        public SubmitSuggestionCommandValidator(IStringLocalizer<Messages> localizer, IImageValidator imageValidator)
        {
            RuleFor(v => v.Title)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.SuggestionMessages.TitleRequired.Value])
                .MaximumLength(200).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 200]);

            RuleFor(v => v.Description)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.SuggestionMessages.DescriptionRequired.Value])
                .MaximumLength(2000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 2000]);

            RuleFor(v => v.Tags)
                .MaximumLength(500).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 500])
                .When(v => v.Tags is not null);

            RuleFor(v => v.ReferenceImage)
                .Must(file => file is null || imageValidator.IsValidImage(file))
                .WithMessage(localizer[LocalizationKeys.UploadFileMessages.FileNotValid.Value]);
        }
    }
}
