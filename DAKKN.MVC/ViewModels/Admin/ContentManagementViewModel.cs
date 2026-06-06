using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class ContentManagementViewModel
    {
        public SectionOrderViewModel SectionOrder { get; set; } = new();
        public HeroSettingsViewModel Hero { get; set; } = new();
        public FeaturedCategoriesViewModel Categories { get; set; } = new();
        public FeaturedProductsViewModel Products { get; set; } = new();
        public GlobalPricingViewModel Pricing { get; set; } = new();
    }

    public class SectionOrderViewModel
    {
        public List<LandingPageSection> Sections { get; set; } = new()
        {
            new LandingPageSection { Id = "hero", Name = "Hero Banner", IsVisible = true, DisplayOrder = 0 },
            new LandingPageSection { Id = "shop", Name = "Featured Products", IsVisible = true, DisplayOrder = 1 },
            new LandingPageSection { Id = "about", Name = "About Us", IsVisible = true, DisplayOrder = 2 },
            new LandingPageSection { Id = "categories", Name = "Categories Slider", IsVisible = true, DisplayOrder = 3 },
            new LandingPageSection { Id = "all-products", Name = "Browse All Products", IsVisible = true, DisplayOrder = 4 },
            new LandingPageSection { Id = "testimonials", Name = "Customer Stories", IsVisible = true, DisplayOrder = 5 },
            new LandingPageSection { Id = "contact", Name = "Contact Form", IsVisible = true, DisplayOrder = 6 }
        };
    }

    public class LandingPageSection
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsVisible { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class HeroSettingsViewModel
    {
        public string Badge { get; set; } = "Industrial Grade Quality";
        public string TitleA { get; set; } = "Quality You Can";
        public string TitleB { get; set; } = "Feel";
        public string TitleC { get; set; } = ", Styles You";
        public string TitleD { get; set; } = "Love";
        public string Description { get; set; } = "Engineered with 5-layer vinyl construction...";
        public string ButtonText { get; set; } = "Shop Best Sellers";
        public string ImageUrl { get; set; } = "https://lh3.googleusercontent.com/...";
    }

    public class FeaturedCategoriesViewModel
    {
        public List<string> SelectedCategoryIds { get; set; } = new();
    }

    public class FeaturedProductsViewModel
    {
        public bool AutoSelectBestSellers { get; set; } = true;
        public List<string> ManualProductIds { get; set; } = new();
    }

    public class GlobalPricingViewModel
    {
        [Display(Name = "Base Price (EGP)")]
        public decimal BasePrice { get; set; } = 70;

        [Display(Name = "Hologram Effect (EGP)")]
        public decimal HologramSurcharge { get; set; } = 10;

        [Display(Name = "Custom Cut (EGP)")]
        public decimal CustomCutSurcharge { get; set; } = 5;

        [Display(Name = "Shipping Rate (EGP)")]
        public decimal ShippingRate { get; set; } = 45;
    }
}
