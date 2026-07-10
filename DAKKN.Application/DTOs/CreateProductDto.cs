using System;
using System.Collections.Generic;

namespace DAKKN.Application.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ArDescription { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();
        public Guid CategoryId { get; set; }
    }

    public class UpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ArDescription { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> FinishOptions { get; set; } = new();
        public List<string> SizeOptions { get; set; } = new();
        public Guid CategoryId { get; set; }
    }
}
