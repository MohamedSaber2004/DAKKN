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
                .EmailAddress().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key]) // Or use a specific email error key if available
                .MustAsync(EmailNotFoundBefore).WithMessage(_localizer[LocalizationKeys.AuthMessages.EmailFoundBefore.Key]); // Or use a specific email already exists error key if available

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
    }
}
