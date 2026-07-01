using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Options;
using DAKKN.Application.Localization;
using DAKKN.Infrastructure.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DAKKN.Tests.Tests.Infrastructure
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<EmailSettings>> _settingsMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly EmailSettings _settings;

        public EmailServiceTests()
        {
            _settings = new EmailSettings
            {
                Email = "noreply@dakkn.com",
                Name = "DAKKN Store",
                Host = "smtp.example.com",
                Port = 587,
                Username = "user",
                Password = "pass",
                ForgetPasswordExpiryMinutes = 30
            };
            _settingsMock = new Mock<IOptions<EmailSettings>>();
            _settingsMock.Setup(x => x.Value).Returns(_settings);

            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(x => x[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, $"[{key}]"));
            _localizerMock.Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()]).Returns((string key, object[] args) => new LocalizedString(key, $"[{key}] {string.Join(",", args)}"));
        }

        private MimeMessage? _capturedMessage;
        private bool _sendCalled;

        private TestableEmailService CreateService()
        {
            _sendCalled = false;
            _capturedMessage = null;
            return new TestableEmailService(_settingsMock.Object, _localizerMock.Object)
            {
                OnSendEmail = msg => { _capturedMessage = msg; _sendCalled = true; }
            };
        }

        [Fact]
        public async Task SendPasswordResetEmailAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendPasswordResetEmailAsync("user@test.com", "Test User", "RESET-TOKEN-123");

            _sendCalled.Should().BeTrue();
            _capturedMessage.Should().NotBeNull();
            _capturedMessage!.Subject.Should().Contain("Password Reset");
            _capturedMessage.To.Mailboxes.Should().ContainSingle(m => m.Address == "user@test.com");
            _capturedMessage.From.Mailboxes.Should().ContainSingle(m => m.Address == "noreply@dakkn.com");
            _capturedMessage.HtmlBody.Should().NotBeNullOrEmpty();
            _capturedMessage.HtmlBody.Should().Contain("RESET-TOKEN-123");
            _capturedMessage.HtmlBody.Should().Contain("Test User");
        }

        [Fact]
        public async Task SendTicketCreatedEmailAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendTicketCreatedEmailAsync("user@test.com", "Test User", "TKT-001", "Help needed");

            _sendCalled.Should().BeTrue();
            _capturedMessage.Should().NotBeNull();
            _capturedMessage!.Subject.Should().Contain("Support Ticket");
            _capturedMessage.HtmlBody.Should().Contain("TKT-001");
            _capturedMessage.HtmlBody.Should().Contain("Help needed");
        }

        [Fact]
        public async Task SendTicketReplyEmailAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendTicketReplyEmailAsync("user@test.com", "Test User", "TKT-001", "Thank you!", false);

            _sendCalled.Should().BeTrue();
            _capturedMessage!.Subject.Should().Contain("TKT-001");
            _capturedMessage.HtmlBody.Should().Contain("Thank you!");
        }

        [Fact]
        public async Task SendTicketAssignedEmailAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendTicketAssignedEmailAsync("admin@test.com", "Admin", "TKT-001", "Support Agent");

            _sendCalled.Should().BeTrue();
            _capturedMessage!.Subject.Should().Contain("TKT-001");
            _capturedMessage.HtmlBody.Should().Contain("Support Agent");
        }

        [Fact]
        public async Task SendTicketClosedEmailAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendTicketClosedEmailAsync("user@test.com", "Test User", "TKT-001");

            _sendCalled.Should().BeTrue();
            _capturedMessage!.Subject.Should().Contain("TKT-001");
            _capturedMessage.HtmlBody.Should().Contain("closed");
        }

        [Fact]
        public async Task SendNewTicketNotificationToAdminAsync_ShouldBuildCorrectEmail()
        {
            var service = CreateService();
            await service.SendNewTicketNotificationToAdminAsync("admin@test.com", "Admin", "TKT-001", "Test User", "Help");

            _sendCalled.Should().BeTrue();
            _capturedMessage!.Subject.Should().Contain("TKT-001");
            _capturedMessage.HtmlBody.Should().Contain("Test User");
        }

        private class TestableEmailService : EmailService
        {
            public Action<MimeMessage>? OnSendEmail { get; set; }

            public TestableEmailService(IOptions<EmailSettings> settings, IStringLocalizer<Messages> localizer)
                : base(settings, localizer) { }

            public override async Task SendEmailAsync(string toEmail, string fullName, string subject, string body, CancellationToken ct)
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("DAKKN Store", "noreply@dakkn.com"));
                email.To.Add(new MailboxAddress(fullName, toEmail));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
                OnSendEmail?.Invoke(email);
                await Task.CompletedTask;
            }
        }
    }
}
