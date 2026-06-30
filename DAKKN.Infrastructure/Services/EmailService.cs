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

        public async Task SendTicketCreatedEmailAsync(string toEmail, string fullName, string ticketNumber, string subject, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subj = isArabic ? "تم إنشاء تذكرة دعم جديدة" : "New Support Ticket Created";
            var body = $@"
                <html><body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    <h2>{fullName}،</h2>
                    <p>{(isArabic ? $"تم إنشاء تذكرة الدعم #{ticketNumber} بنجاح" : $"Your support ticket #{ticketNumber} has been created successfully")}</p>
                    <p>{(isArabic ? $"الموضوع: {subject}" : $"Subject: {subject}")}</p>
                    <p>{(isArabic ? "سنتواصل معك في أقرب وقت ممكن." : "We will get back to you as soon as possible.")}</p>
                    <p>{(isArabic ? "مع تحيات،" : "Best regards,")}<br/>DAKKN</p>
                </body></html>";
            await SendEmailAsync(toEmail, fullName, subj, body, ct);
        }

        public async Task SendTicketReplyEmailAsync(string toEmail, string fullName, string ticketNumber, string replyMessage, bool isStaffReply, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subj = isArabic ? $"رد جديد على التذكرة #{ticketNumber}" : $"New Reply on Ticket #{ticketNumber}";
            var body = $@"
                <html><body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    <h2>{fullName}،</h2>
                    <p>{(isArabic ? $"تمت إضافة رد {(isStaffReply ? "من فريق الدعم" : "منك")} على التذكرة #{ticketNumber}" : $"A reply has been added {(isStaffReply ? "by the support team" : "by you")} to ticket #{ticketNumber}")}</p>
                    <div style='background:#f4f4f4;padding:15px;border-radius:5px;'>{replyMessage}</div>
                    <p>{(isArabic ? "مع تحيات،" : "Best regards,")}<br/>DAKKN</p>
                </body></html>";
            await SendEmailAsync(toEmail, fullName, subj, body, ct);
        }

        public async Task SendTicketAssignedEmailAsync(string toEmail, string fullName, string ticketNumber, string adminName, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subj = isArabic ? $"تم تعيين التذكرة #{ticketNumber}" : $"Ticket #{ticketNumber} Assigned";
            var body = $@"
                <html><body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    <h2>{fullName}،</h2>
                    <p>{(isArabic ? $"تم تعيين التذكرة #{ticketNumber} إلى {adminName}" : $"Ticket #{ticketNumber} has been assigned to {adminName}")}</p>
                    <p>{(isArabic ? "مع تحيات،" : "Best regards,")}<br/>DAKKN</p>
                </body></html>";
            await SendEmailAsync(toEmail, fullName, subj, body, ct);
        }

        public async Task SendTicketClosedEmailAsync(string toEmail, string fullName, string ticketNumber, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subj = isArabic ? $"تم إغلاق التذكرة #{ticketNumber}" : $"Ticket #{ticketNumber} Closed";
            var body = $@"
                <html><body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    <h2>{fullName}،</h2>
                    <p>{(isArabic ? $"تم إغلاق التذكرة #{ticketNumber}" : $"Ticket #{ticketNumber} has been closed")}</p>
                    <p>{(isArabic ? "إذا كان لديك أي استفسارات أخرى، لا تتردد في الاتصال بنا." : "If you have any other questions, please don't hesitate to contact us.")}</p>
                    <p>{(isArabic ? "مع تحيات،" : "Best regards,")}<br/>DAKKN</p>
                </body></html>";
            await SendEmailAsync(toEmail, fullName, subj, body, ct);
        }

        public async Task SendNewTicketNotificationToAdminAsync(string adminEmail, string adminName, string ticketNumber, string customerName, string subject, CancellationToken ct = default)
        {
            var isArabic = CultureInfo.CurrentUICulture.Name.StartsWith("ar");
            var subj = isArabic ? $"تذكرة دعم جديدة #{ticketNumber}" : $"New Support Ticket #{ticketNumber}";
            var body = $@"
                <html><body style='font-family: Arial, sans-serif; direction: {(isArabic ? "rtl" : "ltr")}; text-align: {(isArabic ? "right" : "left")};'>
                    <h2>{adminName}،</h2>
                    <p>{(isArabic ? $"تم إنشاء تذكرة دعم جديدة بواسطة {customerName}" : $"A new support ticket has been created by {customerName}")}</p>
                    <p>{(isArabic ? $"رقم التذكرة: {ticketNumber}" : $"Ticket #: {ticketNumber}")}</p>
                    <p>{(isArabic ? $"الموضوع: {subject}" : $"Subject: {subject}")}</p>
                    <p>{(isArabic ? "يرجى مراجعة وتعيين التذكرة." : "Please review and assign the ticket.")}</p>
                    <p>{(isArabic ? "مع تحيات،" : "Best regards,")}<br/>DAKKN</p>
                </body></html>";
            await SendEmailAsync(adminEmail, adminName, subj, body, ct);
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
