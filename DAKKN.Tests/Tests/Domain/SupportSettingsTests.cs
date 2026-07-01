using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportSettingsTests
{
    [Fact]
    public void CreateDefault_ShouldSetPresetDefaults()
    {
        var settings = SupportSettings.CreateDefault();

        settings.Id.Should().NotBeEmpty();
        settings.SupportEmail.Should().Be("support@dakkn.com");
        settings.DefaultPriority.Should().Be("Medium");
        settings.DefaultResponseTime.Should().Be("24h");
        settings.MaxAttachmentSize.Should().Be(10);
        settings.AllowedExtensions.Should().Be(".jpg,.jpeg,.png,.pdf,.docx,.zip");
        settings.AutoCloseDays.Should().Be(7);
        settings.NotifyOnNewTicket.Should().BeTrue();
        settings.NotifyOnReply.Should().BeTrue();
        settings.NotifyOnAssignment.Should().BeTrue();
        settings.NotifyOnStatusChange.Should().BeTrue();
        settings.IsActive.Should().BeTrue();
        settings.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AllNotifications_ShouldDefaultToTrue()
    {
        var settings = new SupportSettings();
        settings.NotifyOnNewTicket.Should().BeTrue();
        settings.NotifyOnReply.Should().BeTrue();
        settings.NotifyOnAssignment.Should().BeTrue();
        settings.NotifyOnStatusChange.Should().BeTrue();
    }

    [Fact]
    public void DefaultPriority_ShouldBeMedium()
    {
        var settings = new SupportSettings();
        settings.DefaultPriority.Should().Be("Medium");
    }
}
