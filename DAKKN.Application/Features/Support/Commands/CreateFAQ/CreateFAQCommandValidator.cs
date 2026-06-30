using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Support.Commands.CreateFAQ
{
    public class CreateFAQCommandValidator : AbstractValidator<CreateFAQCommand>
    {
        public CreateFAQCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.Question)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(500).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 500]);

            RuleFor(v => v.ArQuestion)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(500).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 500]);

            RuleFor(v => v.Answer)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(5000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 5000]);

            RuleFor(v => v.ArAnswer)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(5000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 5000]);

            RuleFor(v => v.CategoryId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
        }
    }
}
