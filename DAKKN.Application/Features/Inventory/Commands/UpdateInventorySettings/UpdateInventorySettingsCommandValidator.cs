using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings
{
    public class UpdateInventorySettingsCommandValidator : AbstractValidator<UpdateInventorySettingsCommand>
    {
        public UpdateInventorySettingsCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.GlobalDangerQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage(localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 0]);
        }
    }
}
