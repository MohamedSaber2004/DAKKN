using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;

namespace DAKKN.MVC.ViewModels.Landing
{
    public class LandingPageViewModel
    {
        public List<ProductDto> FeaturedProducts { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public PagginatedResult<ProductDto>? AllProducts { get; set; }
        public bool HasFeaturedProducts => FeaturedProducts.Count > 0;
        public bool HasCategories => Categories.Count > 0;
    }
}
