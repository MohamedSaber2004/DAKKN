using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace DAKKN.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken, CancellationToken ct = default)
        {
            var subject = "Dakkn Store - Password Reset Request";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Hello {fullName},</h2>
                    <p>We received a request to reset your password for your Dakkn Store account.</p>
                    <p>Please use the following token to reset your password. This token will expire in {_settings.ForgetPasswordExpiryMinutes} minutes.</p>
                    <div style='background-color: #f4f4f4; padding: 10px; border-radius: 5px; font-weight: bold; font-size: 1.2em; text-align: center;'>
                        {resetToken}
                    </div>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <p>Regards,<br />Dakkn Store Team</p>
                </body>
                </html>";

            await SendEmailAsync(toEmail, fullName, subject, body, ct);
        }

        private async Task SendEmailAsync(string toEmail, string fullName, string subject, string body, CancellationToken ct)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.Name, _settings.Email));
            email.To.Add(new MailboxAddress(fullName, toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            smtp.Timeout = 5000; // 5 seconds timeout
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls, ct);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            await smtp.SendAsync(email, ct);
            await smtp.DisconnectAsync(true, ct);
        }
    }
}
