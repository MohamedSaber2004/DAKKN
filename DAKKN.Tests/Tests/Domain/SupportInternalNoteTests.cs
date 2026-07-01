using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportInternalNoteTests
{
    [Fact]
    public void Create_ShouldInitializeNote()
    {
        var ticketId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var note = SupportInternalNote.Create(ticketId, userId, "Admin User", "This is an internal note about the ticket.");

        note.Id.Should().NotBeEmpty();
        note.TicketId.Should().Be(ticketId);
        note.UserId.Should().Be(userId);
        note.UserName.Should().Be("Admin User");
        note.Note.Should().Be("This is an internal note about the ticket.");
        note.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        note.IsActive.Should().BeTrue();
    }
}
