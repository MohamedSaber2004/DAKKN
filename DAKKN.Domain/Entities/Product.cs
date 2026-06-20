using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAKKN.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ArDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public int QuantityInStock { get; set; } = 0;
        public int DangerQuantity { get; set; } = 0;
        public DateTime? LastStockUpdateDate { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public bool IsInStock => QuantityInStock > 0;

        public ProductStockStatus StockStatus
        {
            get
            {
                if (QuantityInStock == 0) return ProductStockStatus.OutOfStock;
                if (QuantityInStock <= DangerQuantity) return ProductStockStatus.LowStock;
                return ProductStockStatus.InStock;
            }
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            if (QuantityInStock < quantity)
                throw new InvalidOperationException($"Insufficient stock. Available: {QuantityInStock}, Requested: {quantity}");
            QuantityInStock -= quantity;
            LastStockUpdateDate = DateTime.UtcNow;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.", nameof(quantity));
            QuantityInStock += quantity;
            LastStockUpdateDate = DateTime.UtcNow;
        }

        public void SetStock(int quantity, int dangerQuantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.", nameof(quantity));
            if (dangerQuantity < 0) throw new ArgumentException("Danger quantity cannot be negative.", nameof(dangerQuantity));
            QuantityInStock = quantity;
            DangerQuantity = dangerQuantity;
            LastStockUpdateDate = DateTime.UtcNow;
        }
    }
}
