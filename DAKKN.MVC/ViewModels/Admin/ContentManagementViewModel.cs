using DAKKN.Application.Features.BrandReviews.DTOs;
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
        public FaqSettingsViewModel Faq { get; set; } = new();
        public List<BrandReviewDto> BrandReviews { get; set; } = new();
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
            new LandingPageSection { Id = "faq", Name = "FAQ", IsVisible = true, DisplayOrder = 6 },
            new LandingPageSection { Id = "contact", Name = "Contact Form", IsVisible = true, DisplayOrder = 7 }
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
        public string BadgeAr { get; set; } = string.Empty;
        public string TitleAAr { get; set; } = string.Empty;
        public string TitleBAr { get; set; } = string.Empty;
        public string TitleCAr { get; set; } = string.Empty;
        public string TitleDAr { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string ButtonTextAr { get; set; } = string.Empty;

        public string BadgeEn { get; set; } = string.Empty;
        public string TitleAEn { get; set; } = string.Empty;
        public string TitleBEn { get; set; } = string.Empty;
        public string TitleCEn { get; set; } = string.Empty;
        public string TitleDEn { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ButtonTextEn { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = "https://lh3.googleusercontent.com/...";

        public int TrustedCountOverride { get; set; }
    }

    public class AboutSettingsViewModel
    {
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

        public string Feature1TitleAr { get; set; } = string.Empty;
        public string Feature1TitleEn { get; set; } = string.Empty;
        public string Feature1DescAr { get; set; } = string.Empty;
        public string Feature1DescEn { get; set; } = string.Empty;

        public string Feature2TitleAr { get; set; } = string.Empty;
        public string Feature2TitleEn { get; set; } = string.Empty;
        public string Feature2DescAr { get; set; } = string.Empty;
        public string Feature2DescEn { get; set; } = string.Empty;

        public string Feature3TitleAr { get; set; } = string.Empty;
        public string Feature3TitleEn { get; set; } = string.Empty;
        public string Feature3DescAr { get; set; } = string.Empty;
        public string Feature3DescEn { get; set; } = string.Empty;
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
        public string? ProfilePictureUrl { get; set; }
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
        public string Description { get; set; } = string.Empty;
        public List<CategoryManagementItem> AllCategories { get; set; } = new()
        {
            new CategoryManagementItem { Id = "cat-diecut", Name = "Die-Cut", IsFeatured = true, Icon = "cut" },
            new CategoryManagementItem { Id = "cat-holographic", Name = "Holographic", IsFeatured = true, Icon = "flare" },
            new CategoryManagementItem { Id = "cat-clear", Name = "Clear", IsFeatured = false, Icon = "opacity" },
            new CategoryManagementItem { Id = "cat-sheets", Name = "Sticker Sheets", IsFeatured = true, Icon = "layers" },
            new CategoryManagementItem { Id = "cat-labels", Name = "Custom Labels", IsFeatured = false, Icon = "label" },
            new CategoryManagementItem { Id = "cat-matte", Name = "Matte Vinyl", IsFeatured = true, Icon = "texture" }
        };
    }

    public class CategoryManagementItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }

    public class FeaturedProductsViewModel
    {
        public bool AutoSelectBestSellers { get; set; } = true;
        public string SelectedProductIds { get; set; } = string.Empty;
        public List<ProductManagementItem> AllProducts { get; set; } = new();
    }

    public class ProductManagementItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }

    public class FaqSettingsViewModel
    {
        public string TitleAr { get; set; } = "الأسئلة الشائعة";
        public string TitleEn { get; set; } = "Frequently Asked Questions";
        public string DescriptionAr { get; set; } = "إجابات سريعة على الأسئلة الأكثر شيوعاً.";
        public string DescriptionEn { get; set; } = "Quick answers to the most common questions.";
        public int DisplayLimit { get; set; } = 6;
        public bool AutoFetchFromSupport { get; set; } = true;
    }

}
