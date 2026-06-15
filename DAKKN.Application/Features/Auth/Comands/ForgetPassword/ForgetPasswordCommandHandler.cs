using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DAKKN.Application.Features.Auth.Comands.ForgetPassword
{
    public sealed class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Unit>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgetPasswordCommandHandler> _logger;
        private readonly EmailSettings _settings;

        public ForgetPasswordCommandHandler(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<ForgetPasswordCommandHandler> logger,
            IOptions<EmailSettings> settings)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<Unit> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Forget password requested for non-existent email: {Email}", request.Email);
                return Unit.Value;
            }

            var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            _logger.LogInformation("Generated OTP for {Email}: {OTP}", request.Email, otp);

            var otpHash = ComputeSha256Hash(otp);
            user.SetPasswordResetToken(otpHash, DateTime.UtcNow.AddMinutes(_settings.ForgetPasswordExpiryMinutes));
            await _userManager.UpdateAsync(user);

            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FullName, otp, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}. OTP was: {OTP}", request.Email, otp);
                // We don't rethrow here to allow the process to continue in the UI (for security and development ease)
            }

            return Unit.Value;
        }

        internal static string ComputeSha256Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
