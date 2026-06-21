using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;

namespace DAKKN.Domain.Entities
{
    public class Order : BaseEntity<Guid>
    {
        public string OrderNumber { get; private set; } = null!;
        public string TrackingNumber { get; private set; } = null!;
        public Guid? UserId { get; private set; }
        public string CustomerName { get; private set; } = null!;
        public string CustomerEmail { get; private set; } = null!;
        public string CustomerPhone { get; private set; } = null!;
        public string ShippingAddress { get; private set; } = null!;
        public Guid ShippingGovernorateId { get; private set; }
        public string ShippingGovernorateName { get; private set; } = null!;
        public decimal ShippingCost { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; private set; }
        public string? Notes { get; private set; }

        private Order() : base() { }

        public Order(
            string customerName,
            string customerEmail,
            string customerPhone,
            string shippingAddress,
            Guid shippingGovernorateId,
            string shippingGovernorateName,
            decimal shippingCost,
            decimal subtotal,
            Guid? userId,
            string? notes) : base()
        {
            Id = Guid.NewGuid();
            OrderNumber = GenerateOrderNumber();
            TrackingNumber = GenerateTrackingNumber();
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CustomerPhone = customerPhone;
            ShippingAddress = shippingAddress;
            ShippingGovernorateId = shippingGovernorateId;
            ShippingGovernorateName = shippingGovernorateName;
            ShippingCost = shippingCost;
            Subtotal = subtotal;
            TotalAmount = subtotal + shippingCost;
            UserId = userId;
            Notes = notes;
            Status = OrderStatus.Pending;
        }

        public bool CanTransitionTo(OrderStatus target)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled || Status == OrderStatus.Refunded)
                return false;

            if (Status == OrderStatus.Refunded)
                return false;

            if (Status == target)
                return false;

            return true;
        }

        public bool CanCancel()
        {
            return Status != OrderStatus.Delivered
                && Status != OrderStatus.Cancelled
                && Status != OrderStatus.Refunded
                && Status != OrderStatus.Shipped
                && Status != OrderStatus.OutForDelivery;
        }

        public bool CanDelete()
        {
            return Status == OrderStatus.Cancelled || Status == OrderStatus.Refunded;
        }

        public void UpdateStatus(OrderStatus newStatus, string changedBy)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = changedBy;
        }

        public void Cancel(string changedBy)
        {
            Status = OrderStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = changedBy;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }

        private static string GenerateTrackingNumber()
        {
            return $"DKN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }

        public virtual ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
        public virtual ICollection<OrderStatusHistory> StatusHistories { get; private set; } = new List<OrderStatusHistory>();
        public virtual ShippingGovernorate? ShippingGovernorate { get; private set; }
    }
}
