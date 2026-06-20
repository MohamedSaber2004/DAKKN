using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class ShippingGovernorateFormViewModel
    {
        public Guid Id { get; set; }
        public bool IsEdit => Id != Guid.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ArName { get; set; } = string.Empty;

        [Required]
        [Range(0, 9999)]
        public decimal ShippingPrice { get; set; } = 50;

        [Range(0, 999)]
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
