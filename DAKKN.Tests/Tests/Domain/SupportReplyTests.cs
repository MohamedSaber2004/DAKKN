using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportReplyTests
{
    [Fact]
    public void Create_ShouldInitializeReply()
    {
        var ticketId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var reply = SupportReply.Create(ticketId, userId, "John Doe", "Thank you for your help!", true);

        reply.Id.Should().NotBeEmpty();
        reply.TicketId.Should().Be(ticketId);
        reply.UserId.Should().Be(userId);
        reply.UserName.Should().Be("John Doe");
        reply.Message.Should().Be("Thank you for your help!");
        reply.IsStaffReply.Should().BeTrue();
        reply.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        reply.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetIsStaffReply_WhenStaffReplies()
    {
        var userReply = SupportReply.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "Hi", false);
        userReply.IsStaffReply.Should().BeFalse();

        var staffReply = SupportReply.Create(Guid.NewGuid(), Guid.NewGuid(), "Admin", "Hello", true);
        staffReply.IsStaffReply.Should().BeTrue();
    }

    [Fact]
    public void Attachments_ShouldBeEmptyByDefault()
    {
        var reply = SupportReply.Create(Guid.NewGuid(), Guid.NewGuid(), "User", "Message", false);
        reply.Attachments.Should().BeEmpty();
    }

    [Fact]
    public void Attachments_ShouldBeMutable()
    {
        var reply = SupportReply.Create(Guid.NewGuid(), Guid.NewGuid(), "User", "Msg", false);
        var attachment = new SupportAttachment();
        reply.Attachments.Add(attachment);

        reply.Attachments.Should().ContainSingle();
        reply.Attachments.Should().Contain(attachment);
    }
}
