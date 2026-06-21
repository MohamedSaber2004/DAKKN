using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace DAKKN.Application.Features.AccountSecurity.Commands.DeleteAccount
{
    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<DeleteAccountCommandHandler> _logger;

        public DeleteAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer,
            ILogger<DeleteAccountCommandHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());

            if (user == null || user.IsDeleted)
                throw new NotFoundException(_localizer[LocalizationKeys.AuthMessages.UserNotFound.Value]);

            if (!request.ConfirmationText.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Please type DELETE to confirm account deletion.");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordValid)
                throw new BadRequestException(_localizer[LocalizationKeys.Profile.WrongCurrentPassword.Value]);

            user.SoftDelete(_currentUserService.UserId.ToString() ?? "System");
            user.Deactivate();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Account deletion failed for user {UserId}: {Errors}", _currentUserService.UserId, errors);
                throw new BadRequestException(errors);
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("Account deleted successfully for user {UserId}", _currentUserService.UserId);
            return Unit.Value;
        }
    }
}
