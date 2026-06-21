using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Features.Auth.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace DAKKN.Application.Features.Auth.Comands.SignIn
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, AuthResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public SignInCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork unitOfWork,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<AuthResponseDto> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new BadRequestException(_localizer[LocalizationKeys.AuthMessages.InvalidCredentials.Key]);
            }

            if (user.IsDeleted)
            {
                throw new BadRequestException(_localizer[LocalizationKeys.Profile.AccountDeleted.Value]);
            }

            // Generate Access Token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

            // Check for existing valid refresh token
            var existingToken = await _unitOfWork.GetRepository<UserRefreshToken>()
                .GetFirstAsync(x => x.UserId == user.Id && !x.IsRevoked && x.ExpiryDate > DateTime.UtcNow, cancellationToken);

            string refreshToken;
            if (existingToken != null)
            {
                refreshToken = existingToken.Token;
            }
            else
            {
                refreshToken = _jwtTokenService.GenerateRefreshToken(user);
                var userRefreshToken = UserRefreshToken.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));
                await _unitOfWork.GetRepository<UserRefreshToken>().AddAsync(userRefreshToken);
                await _unitOfWork.SaveChangesAsync();
            }

            return new AuthResponseDto(accessToken, refreshToken, user.FullName, user.Email!, user.Id, roles);
        }
    }
}
