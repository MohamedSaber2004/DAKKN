using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Features.Auth.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DAKKN.Application.Features.Auth.Comands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<RefreshTokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var tokenEntity = await _unitOfWork.GetRepository<UserRefreshToken>()
                .GetAllWithIncluding(t => t.Token == request.RefreshToken, t => t.User)
                .FirstOrDefaultAsync(cancellationToken);

            var user = tokenEntity!.User;
            tokenEntity.Revoke();

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user);

            var newTokenEntity = UserRefreshToken.Create(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays));
            await _unitOfWork.GetRepository<UserRefreshToken>().AddAsync(newTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenResponseDto(newAccessToken, newRefreshToken, user.Id);
        }
    }
}
