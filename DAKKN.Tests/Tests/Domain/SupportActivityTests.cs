using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportActivityTests
{
    [Fact]
    public void Create_ShouldInitializeActivity()
    {
        var ticketId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var activity = SupportActivity.Create(ticketId, userId, "John", "StatusChanged", "Updated status", "Open", "InProgress");

        activity.Id.Should().NotBeEmpty();
        activity.TicketId.Should().Be(ticketId);
        activity.UserId.Should().Be(userId);
        activity.UserName.Should().Be("John");
        activity.Action.Should().Be("StatusChanged");
        activity.Details.Should().Be("Updated status");
        activity.OldValue.Should().Be("Open");
        activity.NewValue.Should().Be("InProgress");
        activity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldAllowNullOptionalFields()
    {
        var activity = SupportActivity.Create(Guid.NewGuid(), Guid.NewGuid(), "System", "Created");
        activity.Details.Should().BeNull();
        activity.OldValue.Should().BeNull();
        activity.NewValue.Should().BeNull();
    }
}
