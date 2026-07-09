using DAKKN.MVC.Helpers;
using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.Tests.Tests.Controllers
{
    public class HeroHelperTests
    {
        [Fact]
        public void Deserialize_WithNull_ShouldReturnDefault()
        {
            var result = HeroHelper.Deserialize(null!);
            result.Should().NotBeNull();
            result.BadgeAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithEmptyString_ShouldReturnDefault()
        {
            var result = HeroHelper.Deserialize("");
            result.Should().NotBeNull();
            result.BadgeAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithEmptyObject_ShouldReturnDefault()
        {
            var result = HeroHelper.Deserialize("{}");
            result.Should().NotBeNull();
            result.BadgeAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithModernFormat_ShouldParseCorrectly()
        {
            var json = """
            {
                "badgeAr": "جديد",
                "badgeEn": "New",
                "titleAAr": "عنوان أ",
                "titleAEn": "Title A",
                "descriptionAr": "وصف",
                "descriptionEn": "Description",
                "buttonTextAr": "اضغط",
                "buttonTextEn": "Click",
                "imageUrl": "https://example.com/hero.jpg"
            }
            """;

            var result = HeroHelper.Deserialize(json);
            result.BadgeAr.Should().Be("جديد");
            result.BadgeEn.Should().Be("New");
            result.TitleAAr.Should().Be("عنوان أ");
            result.TitleAEn.Should().Be("Title A");
            result.ButtonTextAr.Should().Be("اضغط");
            result.ButtonTextEn.Should().Be("Click");
            result.ImageUrl.Should().Be("https://example.com/hero.jpg");
        }

        [Fact]
        public void Deserialize_WithLegacyFormat_ShouldMigrate()
        {
            var json = """
            {
                "Badge": "Hot",
                "TitleA": "Big Sale",
                "TitleB": "Up to 50%",
                "TitleC": "Limited Time",
                "TitleD": "Shop Now",
                "Description": "Best deals",
                "ButtonText": "Buy",
                "ImageUrl": "/img.jpg"
            }
            """;

            var result = HeroHelper.Deserialize(json);
            result.BadgeAr.Should().Be("Hot");
            result.BadgeEn.Should().Be("Hot");
            result.TitleAAr.Should().Be("Big Sale");
            result.TitleAEn.Should().Be("Big Sale");
            result.TitleBAr.Should().Be("Up to 50%");
            result.TitleBEn.Should().Be("Up to 50%");
            result.TitleCAr.Should().Be("Limited Time");
            result.TitleCEn.Should().Be("Limited Time");
            result.TitleDAr.Should().Be("Shop Now");
            result.TitleDEn.Should().Be("Shop Now");
            result.ButtonTextAr.Should().Be("Buy");
            result.ButtonTextEn.Should().Be("Buy");
            result.ImageUrl.Should().Be("/img.jpg");
        }

        [Fact]
        public void Deserialize_WithLegacyFormatLowercase_ShouldMigrate()
        {
            var json = """
            {
                "badge": "Offer",
                "titleA": "Sale"
            }
            """;

            var result = HeroHelper.Deserialize(json);
            result.BadgeAr.Should().Be("Offer");
            result.BadgeEn.Should().Be("Offer");
            result.TitleAAr.Should().Be("Sale");
            result.TitleAEn.Should().Be("Sale");
        }

        [Fact]
        public void Deserialize_WithInvalidJson_ShouldReturnDefault()
        {
            var result = HeroHelper.Deserialize("{not valid}");
            result.Should().NotBeNull();
            result.BadgeAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithPartialData_ShouldFillMissing()
        {
            var json = """{ "badgeAr": "Only Badge" }""";
            var result = HeroHelper.Deserialize(json);
            result.BadgeAr.Should().Be("Only Badge");
            result.TitleAAr.Should().BeEmpty();
        }
    }
}
