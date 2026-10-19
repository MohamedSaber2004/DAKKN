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

        /// <summary>
        /// Ordered filenames of the images that should remain on the product. Round-tripped through
        /// a hidden field in the order the admin left them in, so reorder and cover-change are the
        /// same mechanism. The first entry becomes the cover.
        /// </summary>
        public List<string> ExistingImages { get; set; } = new();

        /// <summary>
        /// Filenames the admin removed in this submit. Deleted from disk only after the product
        /// row commits successfully.
        /// </summary>
        public List<string> RemovedImages { get; set; } = new();

        /// <summary>Images newly chosen in this submit. Appended after ExistingImages.</summary>
        public List<IFormFile>? ImageFiles { get; set; }

        /// <summary>
        /// Final display order the admin left the gallery in, one entry per image:
        /// <c>e:filename</c> for an image already on the product, <c>n:index</c> for the
        /// index-th newly uploaded file in <see cref="ImageFiles"/>. Lets the admin reorder and
        /// choose a cover among brand-new uploads, which <see cref="ExistingImages"/> alone
        /// cannot express because those files do not exist on disk yet.
        /// </summary>
        public List<string> ImageOrder { get; set; } = new();

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
        public bool IsFeatured { get; set; } = false;
    }
}
