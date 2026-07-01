using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class OrderStatusHistoryTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var orderId = Guid.NewGuid();

        var history = new OrderStatusHistory(orderId, OrderStatus.Pending, OrderStatus.Confirmed, "admin@test.com", "Order confirmed by admin");

        history.Id.Should().NotBeEmpty();
        history.OrderId.Should().Be(orderId);
        history.OldStatus.Should().Be(OrderStatus.Pending);
        history.NewStatus.Should().Be(OrderStatus.Confirmed);
        history.ChangedBy.Should().Be("admin@test.com");
        history.Notes.Should().Be("Order confirmed by admin");
    }

    [Fact]
    public void Constructor_ShouldAllowNullNotes()
    {
        var history = new OrderStatusHistory(Guid.NewGuid(), OrderStatus.Pending, OrderStatus.Processing, "system");
        history.Notes.Should().BeNull();
    }

    [Fact]
    public void Constructor_ShouldTrackFullStatusTransition()
    {
        var history = new OrderStatusHistory(Guid.NewGuid(), OrderStatus.Processing, OrderStatus.Shipped, "warehouse");
        history.OldStatus.Should().Be(OrderStatus.Processing);
        history.NewStatus.Should().Be(OrderStatus.Shipped);
    }
}
