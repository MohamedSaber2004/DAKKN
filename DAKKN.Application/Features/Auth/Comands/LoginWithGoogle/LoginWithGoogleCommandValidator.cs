using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.LoginWithGoogle
{
    public sealed class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
    {
        public LoginWithGoogleCommandValidator(IStringLocalizer<Messages> localizer)
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage(JsonLocalizationProvider.GetLocalizedString(localizer[LocalizationKeys.AuthMessages.GoogleTokenRequired.Value]));
        }
    }
}
