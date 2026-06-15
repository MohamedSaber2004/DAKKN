using DAKKN.Application.Features.Auth.Comands.ForgetPassword;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DAKKN.Application.Features.Auth.Comands.VerifyForgetPasswordOtp
{
    public sealed class VerifyForgetPasswordOtpCommandHandler : IRequestHandler<VerifyForgetPasswordOtpCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public VerifyForgetPasswordOtpCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(VerifyForgetPasswordOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return false;

            if (user.PasswordResetTokenExpiry < DateTime.UtcNow) return false;

            var submittedHash = ForgetPasswordCommandHandler.ComputeSha256Hash(request.Otp);
            return string.Equals(user.PasswordResetToken, submittedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
