using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;
using System;

namespace DAKKN.Domain.Entities
{
    public class InventoryTransaction : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public int QuantityChanged { get; set; }
        public int PreviousQuantity { get; set; }
        public int NewQuantity { get; set; }
        public InventoryTransactionType TransactionType { get; set; }
        public string? Notes { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
