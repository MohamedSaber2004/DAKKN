using FluentValidation;
using DAKKN.Application.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;
using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandValidator(IStringLocalizer<Messages> localizer, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(v => v.FullName)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .EmailAddress().WithMessage(localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value])
                .MustAsync(BeUniqueEmail).WithMessage(localizer[LocalizationKeys.AuthMessages.EmailFoundBefore.Value]);

            RuleFor(v => v.PhoneNumber)
                .MustAsync(BeUniquePhoneNumber).WithMessage(localizer[LocalizationKeys.AuthMessages.PhoneNumberFoundBefore.Value])
                .When(v => !string.IsNullOrEmpty(v.PhoneNumber));

            RuleFor(v => v.Role)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value]);
            
            RuleFor(v => v.Password)
                .MinimumLength(6).WithMessage(string.Format(localizer[LocalizationKeys.ValidationMessages.MinLength.Value], 6))
                .When(v => !string.IsNullOrWhiteSpace(v.Password));
        }

        private async Task<bool> BeUniqueEmail(UpdateUserCommand model, string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user == null || user.Id == model.Id;
        }

        private async Task<bool> BeUniquePhoneNumber(UpdateUserCommand model, string? phoneNumber, CancellationToken cancellationToken)
        {
            return !await _userManager.Users.AnyAsync(x => x.PhoneNumber == phoneNumber && x.Id != model.Id, cancellationToken);
        }
    }
}
