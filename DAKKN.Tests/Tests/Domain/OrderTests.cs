using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Constructor_ShouldCreatePendingOrder()
        {
            var order = new Order(
                "John Doe", "john@test.com", "123456789",
                "Cairo", Guid.NewGuid(), "Cairo",
                50m, 200m, Guid.NewGuid(), null);

            order.Status.Should().Be(OrderStatus.Pending);
            order.OrderNumber.Should().StartWith("ORD-");
            order.TrackingNumber.Should().StartWith("DKN-");
            order.TotalAmount.Should().Be(250m);
            order.Subtotal.Should().Be(200m);
        }

        [Fact]
        public void CanTransitionTo_ShouldReturnTrue_WhenValidTransition()
        {
            var order = CreateTestOrder();
            order.CanTransitionTo(OrderStatus.Confirmed).Should().BeTrue();
            order.CanTransitionTo(OrderStatus.Processing).Should().BeTrue();
        }

        [Fact]
        public void CanTransitionTo_ShouldReturnFalse_WhenSameStatus()
        {
            var order = CreateTestOrder();
            order.CanTransitionTo(OrderStatus.Pending).Should().BeFalse();
        }

        [Fact]
        public void CanTransitionTo_ShouldReturnFalse_WhenTerminalStatus()
        {
            var order = CreateTestOrder();
            order.UpdateStatus(OrderStatus.Delivered, "admin");
            order.CanTransitionTo(OrderStatus.Confirmed).Should().BeFalse();

            var cancelled = CreateTestOrder();
            cancelled.UpdateStatus(OrderStatus.Cancelled, "admin");
            cancelled.CanTransitionTo(OrderStatus.Pending).Should().BeFalse();

            var refunded = CreateTestOrder();
            refunded.UpdateStatus(OrderStatus.Refunded, "admin");
            refunded.CanTransitionTo(OrderStatus.Pending).Should().BeFalse();
        }

        [Fact]
        public void CanCancel_ShouldReturnTrue_ForPendingOrConfirmed()
        {
            var pending = CreateTestOrder();
            pending.CanCancel().Should().BeTrue();

            var confirmed = CreateTestOrder();
            confirmed.UpdateStatus(OrderStatus.Confirmed, "admin");
            confirmed.CanCancel().Should().BeTrue();
        }

        [Fact]
        public void CanCancel_ShouldReturnFalse_WhenShippedOrDelivered()
        {
            var shipped = CreateTestOrder();
            shipped.UpdateStatus(OrderStatus.Shipped, "admin");
            shipped.CanCancel().Should().BeFalse();

            var delivered = CreateTestOrder();
            delivered.UpdateStatus(OrderStatus.Delivered, "admin");
            delivered.CanCancel().Should().BeFalse();
        }

        [Fact]
        public void CanDelete_ShouldReturnTrue_OnlyForCancelledOrRefunded()
        {
            var cancelled = CreateTestOrder();
            cancelled.UpdateStatus(OrderStatus.Cancelled, "admin");
            cancelled.CanDelete().Should().BeTrue();

            var refunded = CreateTestOrder();
            refunded.UpdateStatus(OrderStatus.Refunded, "admin");
            refunded.CanDelete().Should().BeTrue();

            var pending = CreateTestOrder();
            pending.CanDelete().Should().BeFalse();
        }

        [Fact]
        public void Cancel_ShouldSetStatusToCancelled()
        {
            var order = CreateTestOrder();
            order.Cancel("admin");
            order.Status.Should().Be(OrderStatus.Cancelled);
            order.UpdatedBy.Should().Be("admin");
        }

        private static Order CreateTestOrder()
        {
            return new Order(
                "John Doe", "john@test.com", "123456789",
                "Cairo", Guid.NewGuid(), "Cairo",
                50m, 200m, Guid.NewGuid(), null);
        }
    }
}
