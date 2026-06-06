using System.ComponentModel.DataAnnotations;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class ContentManagementViewModel
    {
        public SectionOrderViewModel SectionOrder { get; set; } = new();
        public HeroSettingsViewModel Hero { get; set; } = new();
        public FeaturedCategoriesViewModel Categories { get; set; } = new();
        public FeaturedProductsViewModel Products { get; set; } = new();
        public AboutSettingsViewModel About { get; set; } = new();
        public TestimonialsSettingsViewModel Testimonials { get; set; } = new();
        public ContactSettingsViewModel Contact { get; set; } = new();
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

    public class AboutSettingsViewModel
    {
        public string Title { get; set; } = "Engineered for Extremes";
        public string Description { get; set; } = "Our stickers aren't just printed; they are manufactured...";
        
        public string Feature1Title { get; set; } = "5-Layer Construction";
        public string Feature1Desc { get; set; } = "From the strong adhesive base...";
        
        public string Feature2Title { get; set; } = "Vibrant Customization";
        public string Feature2Desc { get; set; } = "High-fidelity 8-color printing...";
        
        public string Feature3Title { get; set; } = "Industrial Durability";
        public string Feature3Desc { get; set; } = "Dishwasher safe, weather-proof...";
    }

    public class TestimonialsSettingsViewModel
    {
        public string Title { get; set; } = "Community Stories";
        public string Description { get; set; } = "See how our stickers hold up in the real world.";
        public List<TestimonialItem> Reviews { get; set; } = new()
        {
            new TestimonialItem { Name = "Sarah J.", Role = "Snowboarder", Quote = "These stickers have survived 6 months...", Rating = 5, IsFeatured = true },
            new TestimonialItem { Name = "Marcus T.", Role = "Cafe Owner", Quote = "I use them to brand my custom coffee tumblers...", Rating = 5, IsFeatured = true }
        };
    }

    public class TestimonialItem
    {
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Quote { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class ContactSettingsViewModel
    {
        public string Title { get; set; } = "Get in Touch";
        public string Description { get; set; } = "Have a custom bulk order or need support?";
        public string Email { get; set; } = "support@dakkn.com";
        public string Phone { get; set; } = "+20 123 456 7890";
        public string Address { get; set; } = "Damietta, Egypt";
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
