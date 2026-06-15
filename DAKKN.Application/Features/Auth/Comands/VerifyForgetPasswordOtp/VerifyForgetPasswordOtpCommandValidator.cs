using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.VerifyForgetPasswordOtp
{
    public class VerifyForgetPasswordOtpCommandValidator : AbstractValidator<VerifyForgetPasswordOtpCommand>
    {
        public VerifyForgetPasswordOtpCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .EmailAddress().WithMessage(localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value]);
            RuleFor(x => x.Otp)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .Length(6).WithMessage(localizer[LocalizationKeys.AuthMessages.ResetTokenInvalid.Value])
                .Matches(@"^\d{6}$").WithMessage(localizer[LocalizationKeys.AuthMessages.ResetTokenInvalid.Value]);
        }
    }
}
