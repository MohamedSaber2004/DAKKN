using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Localization;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Globalization;

namespace DAKKN.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IStringLocalizer<Messages> _localizer;

        public EmailService(IOptions<EmailSettings> settings, IStringLocalizer<Messages> localizer)
        {
            _settings = settings.Value;
            _localizer = localizer;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subject = isArabic
                ? _localizer["email.reset_password_subject"]
                : "Dakkn Store - Password Reset Request";

            var greeting = isArabic
                ? $"<h2>{_localizer["email.greeting"]} {fullName}،</h2>"
                : $"<h2>{_localizer["email.greeting"]} {fullName},</h2>";

            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    {greeting}
                    <p>{_localizer["email.reset_request_body"]}</p>
                    <p>{_localizer["email.reset_token_expiry", _settings.ForgetPasswordExpiryMinutes]}</p>
                    <div style='background-color: #f4f4f4; padding: 10px; border-radius: 5px; font-weight: bold; font-size: 1.2em; text-align: center; letter-spacing: 3px;'>
                        {resetToken}
                    </div>
                    <p>{_localizer["email.reset_ignore"]}</p>
                    <p>{_localizer["email.regards"]}<br />{_localizer["email.team"]}</p>
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
