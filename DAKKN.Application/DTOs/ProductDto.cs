using System;
using System.Collections.Generic;

namespace DAKKN.Application.DTOs
{
    public class ProductDto
    {
        /// <summary>
        /// Hard cap on images per product. Enforced by the create/update validators and
        /// reflected in the admin form's file input and the customer gallery's layout.
        /// </summary>
        public const int MaxImages = 8;

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

        /// <summary>
        /// Ordered image list, each entry resolved to an absolute path by the query handler.
        /// The first entry is always the cover and equals <see cref="ImageUrl"/>.
        /// Only populated by <c>GetProductById</c>; list/grid queries leave this empty on purpose
        /// so cards do not pay to deserialize the JSON blob.
        /// </summary>
        public List<string> Images { get; set; } = new();
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
        public bool IsFeatured { get; set; }
    }
}
