using DAKKN.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class AddProductViewModel
    {
        public Guid? Id { get; set; }

        public bool IsEdit => Id.HasValue && Id.Value != Guid.Empty;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ArName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ArDescription { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public List<CategoryDto> AvailableCategories { get; set; } = new();

        public List<string> SizeOptions { get; set; } = new();

        public int QuantityInStock { get; set; } = 0;
        public int DangerQuantity { get; set; } = 0;
    }
}
