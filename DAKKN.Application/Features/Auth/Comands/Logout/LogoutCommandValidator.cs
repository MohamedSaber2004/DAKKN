using DAKKN.Application.Localization;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Comands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public LogoutCommandValidator(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;
            
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage(_localizer[LocalizationKeys.AuthMessages.RefreshTokenRequired.Value]);
        }
    }
}
