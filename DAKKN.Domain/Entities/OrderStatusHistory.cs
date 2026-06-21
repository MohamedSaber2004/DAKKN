using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;

namespace DAKKN.Domain.Entities
{
    public class OrderStatusHistory : BaseEntity<Guid>
    {
        public Guid OrderId { get; private set; }
        public OrderStatus OldStatus { get; private set; }
        public OrderStatus NewStatus { get; private set; }
        public string ChangedBy { get; private set; } = null!;
        public string? Notes { get; private set; }

        private OrderStatusHistory() : base() { }

        public OrderStatusHistory(Guid orderId, OrderStatus oldStatus, OrderStatus newStatus, string changedBy, string? notes = null) : base()
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedBy = changedBy;
            Notes = notes;
        }

        public virtual Order Order { get; private set; } = null!;
    }
}
