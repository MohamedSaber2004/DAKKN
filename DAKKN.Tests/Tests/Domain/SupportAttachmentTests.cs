using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportAttachmentTests
{
    [Fact]
    public void Create_ShouldInitializeAttachment()
    {
        var ticketId = Guid.NewGuid();
        var replyId = Guid.NewGuid();

        var attachment = SupportAttachment.Create(ticketId, "file_123.pdf", "report.pdf", "application/pdf", 204800, "/uploads/support/file_123.pdf", replyId);

        attachment.Id.Should().NotBeEmpty();
        attachment.TicketId.Should().Be(ticketId);
        attachment.ReplyId.Should().Be(replyId);
        attachment.FileName.Should().Be("file_123.pdf");
        attachment.OriginalFileName.Should().Be("report.pdf");
        attachment.ContentType.Should().Be("application/pdf");
        attachment.FileSize.Should().Be(204800);
        attachment.FilePath.Should().Be("/uploads/support/file_123.pdf");
        attachment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        attachment.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldAllowNullReplyId()
    {
        var attachment = SupportAttachment.Create(Guid.NewGuid(), "img.jpg", "image.jpg", "image/jpeg", 50000, "/path");
        attachment.ReplyId.Should().BeNull();
    }
}
