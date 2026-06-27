using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.SignUp
{
    public class SignUpCommandValidator : AbstractValidator<SignupCommand>
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public SignUpCommandValidator(IStringLocalizer<Messages> localizer, UserManager<ApplicationUser> userManager)
        {
            _localizer = localizer;
            _userManager = userManager;

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key]);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .EmailAddress().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MustAsync(EmailNotFoundBefore).WithMessage(_localizer[LocalizationKeys.AuthMessages.EmailFoundBefore.Key]);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .Matches(@"^0[0-9]{10,15}$").WithMessage(_localizer[LocalizationKeys.AuthMessages.InvalidPhoneNumber.Value])
                .MustAsync(PhoneNumberNotFoundBefore).WithMessage(_localizer[LocalizationKeys.AuthMessages.PhoneNumberFoundBefore.Key]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .MinimumLength(8).WithMessage(_localizer[LocalizationKeys.ValidationMessages.MinLength.Key, 8]);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .Equal(x => x.Password).WithMessage(_localizer[LocalizationKeys.AuthMessages.PasswordMismatch.Key]);
        }

        private async Task<bool> EmailNotFoundBefore(string email, CancellationToken cancellationToken)
        {
            return !await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        private async Task<bool> PhoneNumberNotFoundBefore(string phoneNumber, CancellationToken cancellationToken)
        {
            return !await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }
    }
}
