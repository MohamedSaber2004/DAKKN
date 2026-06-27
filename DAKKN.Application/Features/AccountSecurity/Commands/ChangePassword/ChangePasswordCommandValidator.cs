using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.AccountSecurity.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MinimumLength(8).WithMessage(localizer[LocalizationKeys.ValidationMessages.MinLength.Value, 8])
                .Matches("[A-Z]").WithMessage(localizer[LocalizationKeys.Profile.PasswordRequirements.Value])
                .Matches("[a-z]").WithMessage(localizer[LocalizationKeys.Profile.PasswordRequirements.Value])
                .Matches("[0-9]").WithMessage(localizer[LocalizationKeys.Profile.PasswordRequirements.Value]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .Equal(x => x.NewPassword).WithMessage(localizer[LocalizationKeys.AuthMessages.PasswordMismatch.Value]);
        }
    }
}
