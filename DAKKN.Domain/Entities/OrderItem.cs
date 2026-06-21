using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class OrderItem : BaseEntity<Guid>
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = null!;
        public string? ProductImageUrl { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalPrice { get; private set; }

        private OrderItem() : base() { }

        public OrderItem(Guid orderId, Guid productId, string productName, string? productImageUrl, decimal unitPrice, int quantity) : base()
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            ProductName = productName;
            ProductImageUrl = productImageUrl;
            UnitPrice = unitPrice;
            Quantity = quantity;
            TotalPrice = unitPrice * quantity;
        }

        public virtual Order Order { get; private set; } = null!;
        public virtual Product Product { get; private set; } = null!;
    }
}
