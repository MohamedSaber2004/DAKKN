using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(v => v.CategoryName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(150).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 150]);

            RuleFor(v => v.ArName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(150).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 150]);
        }
    }
}
