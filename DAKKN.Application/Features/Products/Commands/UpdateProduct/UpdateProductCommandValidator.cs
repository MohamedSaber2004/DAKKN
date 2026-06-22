using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandValidator(IStringLocalizer<Messages> localizer, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(v => v.Id)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Name)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(200).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 200]);

            RuleFor(v => v.ArName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(200).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 200]);

            RuleFor(v => v.Description)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(3000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 3000]);

            RuleFor(v => v.ArDescription)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MaximumLength(3000).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 3000]);

            RuleFor(v => v.Price)
                .GreaterThan(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 0]);

            RuleFor(v => v.ImageUrl)
                .MaximumLength(500).WithMessage(localizer[LocalizationKeys.ValidationMessages.MaxLength.Value, 500]);

            RuleFor(v => v.CategoryId)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MustAsync(async (id, ct) =>
                {
                    var repo = _unitOfWork.GetRepository<Category>();
                    return await repo.ExistsByKeyAsync(id, ct);
                }).WithMessage(localizer[LocalizationKeys.Products.CategoryNotFound.Value]);

            RuleFor(v => v.QuantityInStock)
                .GreaterThanOrEqualTo(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 0]);

            RuleFor(v => v.DangerQuantity)
                .GreaterThanOrEqualTo(0).WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 0]);
        }
    }
}
