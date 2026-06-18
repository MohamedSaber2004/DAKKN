using DAKKN.Application.DTOs;
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

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; }

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public List<CategoryDto> AvailableCategories { get; set; } = new();
    }
}
