using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class SupportTicketTests
{
    [Fact]
    public void Create_ShouldInitializeTicketAsOpen()
    {
        var customerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var ticket = SupportTicket.Create(customerId, "John Doe", "john@test.com",
            "Cannot login", "I am unable to access my account.", categoryId,
            SupportTicketPriority.High, "0123456789", "website", "ORD-123");

        ticket.Id.Should().NotBeEmpty();
        ticket.TicketNumber.Should().StartWith("TK-");
        ticket.CustomerId.Should().Be(customerId);
        ticket.CustomerName.Should().Be("John Doe");
        ticket.CustomerEmail.Should().Be("john@test.com");
        ticket.CustomerPhone.Should().Be("0123456789");
        ticket.Subject.Should().Be("Cannot login");
        ticket.Message.Should().Be("I am unable to access my account.");
        ticket.CategoryId.Should().Be(categoryId);
        ticket.Priority.Should().Be(SupportTicketPriority.High);
        ticket.Status.Should().Be(SupportTicketStatus.Open);
        ticket.Source.Should().Be("website");
        ticket.OrderNumber.Should().Be("ORD-123");
        ticket.IsActive.Should().BeTrue();
        ticket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldDefaultPriorityToMedium()
    {
        var ticket = SupportTicket.Create(Guid.NewGuid(), "Jane", "j@t.com", "Issue", "Details", Guid.NewGuid());
        ticket.Priority.Should().Be(SupportTicketPriority.Medium);
    }

    [Fact]
    public void Create_ShouldAllowNullOptionalFields()
    {
        var ticket = SupportTicket.Create(Guid.NewGuid(), "Jane", "j@t.com", "Issue", "Details", Guid.NewGuid());
        ticket.CustomerPhone.Should().BeNull();
        ticket.Source.Should().BeNull();
        ticket.OrderNumber.Should().BeNull();
    }

    [Fact]
    public void AssignTo_ShouldSetAssignmentAndSetInProgress()
    {
        var ticket = CreateOpenTicket();
        var adminId = Guid.NewGuid();

        ticket.AssignTo(adminId, "Admin User");

        ticket.AssignedToId.Should().Be(adminId);
        ticket.AssignedToName.Should().Be("Admin User");
        ticket.Status.Should().Be(SupportTicketStatus.InProgress);
    }

    [Fact]
    public void ReassignTo_ShouldChangeAssignmentWithoutStatusChange()
    {
        var ticket = CreateOpenTicket();
        ticket.AssignTo(Guid.NewGuid(), "Admin 1");

        ticket.ReassignTo(Guid.NewGuid(), "Admin 2");

        ticket.AssignedToName.Should().Be("Admin 2");
    }

    [Fact]
    public void UpdateStatus_ShouldChangeStatus()
    {
        var ticket = CreateOpenTicket();
        ticket.UpdateStatus(SupportTicketStatus.InProgress);
        ticket.Status.Should().Be(SupportTicketStatus.InProgress);
    }

    [Fact]
    public void UpdateStatus_ShouldSetResolvedAt_WhenResolved()
    {
        var ticket = CreateOpenTicket();
        ticket.UpdateStatus(SupportTicketStatus.Resolved);

        ticket.ResolvedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ticket.Status.Should().Be(SupportTicketStatus.Resolved);
    }

    [Fact]
    public void UpdateStatus_ShouldSetClosedAt_WhenClosed()
    {
        var ticket = CreateOpenTicket();
        ticket.UpdateStatus(SupportTicketStatus.Closed);

        ticket.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        ticket.Status.Should().Be(SupportTicketStatus.Closed);
    }

    [Fact]
    public void UpdatePriority_ShouldChangePriority()
    {
        var ticket = CreateOpenTicket();
        ticket.UpdatePriority(SupportTicketPriority.Urgent);

        ticket.Priority.Should().Be(SupportTicketPriority.Urgent);
    }

    [Fact]
    public void RecordFirstResponse_ShouldSetTimestamp_WhenNull()
    {
        var ticket = CreateOpenTicket();
        ticket.RecordFirstResponse();

        ticket.FirstResponseAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RecordFirstResponse_ShouldNotOverwrite_WhenAlreadySet()
    {
        var ticket = CreateOpenTicket();
        ticket.RecordFirstResponse();
        var first = ticket.FirstResponseAt;

        Thread.Sleep(10);
        ticket.RecordFirstResponse();

        ticket.FirstResponseAt.Should().Be(first);
    }

    private static SupportTicket CreateOpenTicket()
    {
        return SupportTicket.Create(Guid.NewGuid(), "Test", "t@t.com", "Subject", "Message", Guid.NewGuid());
    }
}
