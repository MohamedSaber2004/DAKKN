using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.CategoryName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(150).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 150]);

            RuleFor(v => v.ArName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(150).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 150]);
        }
    }
}
