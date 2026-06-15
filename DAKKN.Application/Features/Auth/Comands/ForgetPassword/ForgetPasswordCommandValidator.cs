using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgetPasswordCommandValidator(IStringLocalizer<Messages> localizer, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(localizer[LocalizationKeys.ValidationMessages.Required.Value])
                .EmailAddress().WithMessage(localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value])
                .MustAsync(UserExists).WithMessage(localizer[LocalizationKeys.AuthMessages.UserNotFound.Value]);
        }

        private async Task<bool> UserExists(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }
    }
}
