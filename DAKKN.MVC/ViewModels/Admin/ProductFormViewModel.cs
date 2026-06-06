using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class AddProductViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000000)]
        public decimal Price { get; set; }

        public string Sku { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public int Quantity { get; set; }

        public bool TrackInventory { get; set; } = true;

        public string Tags { get; set; } = string.Empty;

        public List<string> SelectedMaterials { get; set; } = new();
        public List<string> SelectedSizes { get; set; } = new();

        // Options for selects
        public List<string> AvailableCategories { get; set; } = new() { "Anime", "Games", "Memes", "Tech" };
        public List<string> AvailableMaterials { get; set; } = new() { "Glossy", "Matte", "Holographic" };
        public List<string> AvailableSizes { get; set; } = new() { "5x5", "8x8", "12x12" };
    }
}
