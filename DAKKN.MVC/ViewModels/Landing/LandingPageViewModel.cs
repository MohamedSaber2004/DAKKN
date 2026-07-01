using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;
using DAKKN.MVC.ViewModels.Admin;
using DAKKN.Application.Features.Support.DTOs;

namespace DAKKN.MVC.ViewModels.Landing
{
    public class LandingPageViewModel
    {
        public List<ProductDto> FeaturedProducts { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();
        public PagginatedResult<ProductDto>? AllProducts { get; set; }
        public bool HasFeaturedProducts => FeaturedProducts.Count > 0;
        public bool HasCategories => Categories.Count > 0;

        public HeroSettingsViewModel Hero { get; set; } = new();
        public AboutSettingsViewModel About { get; set; } = new();
        public TestimonialsSettingsViewModel Testimonials { get; set; } = new();
        public ContactSettingsViewModel Contact { get; set; } = new();
        public FaqSettingsViewModel Faq { get; set; } = new();
        public List<SupportFAQDto> FaqItems { get; set; } = new();
        public bool HasCmsData { get; set; }
        public HashSet<string> VisibleSections { get; set; } = new();
        public List<string> OrderedSectionIds { get; set; } = new();
        public string BaseUrl { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }

        public int EffectiveTrustedCount => Hero.TrustedCountOverride > 0 ? Hero.TrustedCountOverride : TotalCustomers;
    }
}
