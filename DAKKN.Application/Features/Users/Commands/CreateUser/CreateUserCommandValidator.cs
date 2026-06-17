using FluentValidation;
using DAKKN.Application.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateUserCommandValidator(IStringLocalizer<Messages> localizer, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(v => v.FullName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .EmailAddress().WithMessage(localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value])
                .MustAsync(BeUniqueEmail).WithMessage(localizer[LocalizationKeys.AuthMessages.EmailFoundBefore.Value]);

            RuleFor(v => v.PhoneNumber)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MustAsync(BeUniquePhoneNumber).WithMessage(localizer[LocalizationKeys.AuthMessages.PhoneNumberFoundBefore.Value]);

            RuleFor(v => v.Password)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .MinimumLength(6).WithMessage(string.Format(localizer[LocalizationKeys.ValidationMessages.MinLength.Value], 6));

            RuleFor(v => v.Role)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
        }

        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            return await _userManager.FindByEmailAsync(email) == null;
        }

        private async Task<bool> BeUniquePhoneNumber(string phoneNumber, CancellationToken cancellationToken)
        {
            return !await _userManager.Users.AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
        }
    }
}
