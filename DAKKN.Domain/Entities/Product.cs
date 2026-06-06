using System;
using System.Collections.Generic;

namespace DAKKN.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();
        public List<ProductFeature> Features { get; set; } = new();
    }

    public class ProductFeature
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
