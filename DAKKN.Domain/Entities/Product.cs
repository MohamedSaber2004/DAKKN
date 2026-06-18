using DAKKN.Domain.Common;
using System;
using System.Collections.Generic;

namespace DAKKN.Domain.Entities
{
    public class Product : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
    }
}
