using DAKKN.Application.Features.Auth.Comands.ForgetPassword;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<Messages> _localizer;

        public ResetPasswordCommandValidator(IStringLocalizer<Messages> localizer,UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _localizer = localizer;
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .EmailAddress().WithMessage(_localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value]);

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .Length(6).WithMessage(_localizer[LocalizationKeys.AuthMessages.ResetTokenInvalid.Value])
                .Matches(@"^\d{6}$").WithMessage(_localizer[LocalizationKeys.AuthMessages.ResetTokenInvalid.Value]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MinimumLength(8).WithMessage(_localizer[LocalizationKeys.ValidationMessages.MinLength.Value])
                .Matches(@"[A-Z]").WithMessage(_localizer[LocalizationKeys.AuthMessages.WeakPassword.Value])
                .Matches(@"[0-9]").WithMessage(_localizer[LocalizationKeys.AuthMessages.WeakPassword.Value])
                .MustAsync(async (command, newPassword, cancellationToken) =>
                {
                    var user = await _userManager.FindByEmailAsync(command.Email);
                    if (user == null) return true;
                    return !await _userManager.CheckPasswordAsync(user, newPassword);
                }).WithMessage(_localizer[LocalizationKeys.AuthMessages.PasswordSameAsOld.Value]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .Equal(x => x.NewPassword).WithMessage(_localizer[LocalizationKeys.AuthMessages.PasswordMismatch.Value]);
            RuleFor(x => x)
                .MustAsync(BeValidOtp).WithMessage(_localizer[LocalizationKeys.AuthMessages.ResetTokenInvalid.Value]);
        }

        private async Task<bool> BeValidOtp(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow) return false;

            var submittedHash = ForgetPasswordCommandHandler.ComputeSha256Hash(request.Token);
            return string.Equals(user.PasswordResetToken, submittedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
