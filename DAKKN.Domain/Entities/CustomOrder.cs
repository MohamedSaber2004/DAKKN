using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;

namespace DAKKN.Domain.Entities
{
    public class CustomOrder : BaseEntity<Guid>
    {
        public string CustomerName { get; private set; } = null!;
        public string CustomerPhone { get; private set; } = null!;
        public string ShippingAddress { get; private set; } = null!;
        public string? Notes { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Shape { get; private set; }
        public string? Size { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalAmount { get; private set; }
        public CustomOrderStatus Status { get; private set; }
        public Guid? UserId { get; private set; }

        private CustomOrder() : base() { }

        public CustomOrder(
            string customerName,
            string customerPhone,
            string shippingAddress,
            string? notes,
            string? imageUrl,
            string? shape,
            string? size,
            int quantity,
            decimal totalAmount,
            Guid? userId) : base()
        {
            Id = Guid.NewGuid();
            CustomerName = customerName;
            CustomerPhone = customerPhone;
            ShippingAddress = shippingAddress;
            Notes = notes;
            ImageUrl = imageUrl;
            Shape = shape;
            Size = size;
            Quantity = quantity;
            TotalAmount = totalAmount;
            UserId = userId;
            Status = CustomOrderStatus.Pending;
        }

        public void Approve()
        {
            Status = CustomOrderStatus.Approved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject()
        {
            Status = CustomOrderStatus.Rejected;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
