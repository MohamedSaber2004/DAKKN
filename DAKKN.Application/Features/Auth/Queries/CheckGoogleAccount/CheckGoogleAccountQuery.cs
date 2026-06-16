using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Auth.Queries.CheckGoogleAccount
{
    public sealed record CheckGoogleAccountQuery(string IdToken) : IRequest<CheckGoogleAccountResponseDto>;

    public sealed record CheckGoogleAccountResponseDto(bool NeedsPhoneNumber, string? Email, string? FullName);

    public sealed class CheckGoogleAccountQueryHandler : IRequestHandler<CheckGoogleAccountQuery, CheckGoogleAccountResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGoogleAuth _googleAuth;
        private readonly IStringLocalizer<Messages> _localizer;

        public CheckGoogleAccountQueryHandler(
            UserManager<ApplicationUser> userManager,
            IGoogleAuth googleAuth,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _googleAuth = googleAuth;
            _localizer = localizer;
        }

        public async Task<CheckGoogleAccountResponseDto> Handle(CheckGoogleAccountQuery request, CancellationToken cancellationToken)
        {
            var payload = await _googleAuth.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);
            if (payload == null)
                throw new UnAuthorizedException(JsonLocalizationProvider.GetLocalizedString(_localizer[LocalizationKeys.AuthMessages.InvalidGoogleToken.Value]));

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.GoogleUserId == payload.Subject, cancellationToken);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
            }

            bool needsPhoneNumber = user == null || string.IsNullOrEmpty(user.PhoneNumber);

            return new CheckGoogleAccountResponseDto(needsPhoneNumber, payload.Email, payload.Name);
        }
    }
}
