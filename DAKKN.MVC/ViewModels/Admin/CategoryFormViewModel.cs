using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class CategoryFormViewModel
    {
        public Guid? Id { get; set; }

        public bool IsEdit => Id.HasValue && Id.Value != Guid.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string ArName { get; set; } = string.Empty;

        public string? ExistingImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
