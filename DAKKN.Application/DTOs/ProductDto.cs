using System;
using System.Collections.Generic;

namespace DAKKN.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ArDescription { get; set; }
        public decimal Price { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? ImageFullUrl { get; set; }
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryArName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int QuantityInStock { get; set; }
        public int DangerQuantity { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
    }
}
