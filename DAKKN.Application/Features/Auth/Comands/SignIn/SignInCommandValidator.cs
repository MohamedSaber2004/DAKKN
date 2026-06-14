using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.SignIn
{
    public class SignInCommandValidator: AbstractValidator<SignInCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<Messages> _localizer;

        public SignInCommandValidator(UserManager<ApplicationUser> userManager, IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key])
                .EmailAddress().WithMessage(_localizer[LocalizationKeys.AuthMessages.InvalidEmail.Key]);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.ValidationMessages.Required.Key]);
            RuleFor(x => x)
                .CustomAsync(async (request, context, cancellationToken) =>
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    {
                        context.AddFailure(_localizer[LocalizationKeys.AuthMessages.InvalidCredentials.Key]);
                    }
                });
        }
    }
}
