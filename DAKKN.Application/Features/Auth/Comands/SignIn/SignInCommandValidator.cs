using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.SignIn
{
    internal class SignInCommandValidator: AbstractValidator<SignInCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<Messages> _localizer;

        public SignInCommandValidator(UserManager<ApplicationUser> userManager, IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(JsonLocalizationProvider.GetLocalizedString(localizer[LocalizationKeys.ValidationMessages.Required.Value]))
                .EmailAddress().WithMessage(JsonLocalizationProvider.GetLocalizedString(localizer[LocalizationKeys.AuthMessages.InvalidEmail.Value]));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(JsonLocalizationProvider.GetLocalizedString(localizer[LocalizationKeys.ValidationMessages.Required.Value]));
            RuleFor(x => x)
                .CustomAsync(async (request, context, cancellationToken) =>
                {
                    var user = await _userManager.FindByEmailAsync(request.Email);
                    if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    {
                        context.AddFailure(JsonLocalizationProvider.GetLocalizedString(localizer[LocalizationKeys.AuthMessages.InvalidCredentials.Value]));
                    }
                });
        }
    }
}
