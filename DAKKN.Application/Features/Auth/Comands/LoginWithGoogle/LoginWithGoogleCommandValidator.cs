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

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^0[0-9]{10,15}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage(localizer[LocalizationKeys.AuthMessages.InvalidPhoneNumber.Value]);
        }
    }
}
