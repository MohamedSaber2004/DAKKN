using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DAKKN.Application.Features.AccountSecurity.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer,
            ILogger<ChangePasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());

            if (user == null || user.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.AuthMessages.UserNotFound.Value]);

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isCurrentPasswordValid)
                throw new BadRequestException(_localizer[LocalizationKeys.Profile.WrongCurrentPassword.Value]);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password change failed for user {UserId}: {Errors}", _currentUserService.UserId, errors);
                throw new BadRequestException(errors);
            }

            _logger.LogInformation("Password changed successfully for user {UserId}", _currentUserService.UserId);
            return Unit.Value;
        }
    }
}
