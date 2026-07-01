using DAKKN.MVC.Helpers;
using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.Tests.Tests.Controllers
{
    public class AboutHelperTests
    {
        [Fact]
        public void Deserialize_WithNull_ShouldReturnDefault()
        {
            var result = AboutHelper.Deserialize(null!);
            result.Should().NotBeNull();
            result.TitleAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithEmptyString_ShouldReturnDefault()
        {
            var result = AboutHelper.Deserialize("");
            result.Should().NotBeNull();
            result.TitleAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithEmptyObject_ShouldReturnDefault()
        {
            var result = AboutHelper.Deserialize("{}");
            result.Should().NotBeNull();
            result.TitleAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithModernFormat_ShouldParseCorrectly()
        {
            var json = """
            {
                "titleAr": "عن المتجر",
                "titleEn": "About Store",
                "descriptionAr": "وصف المتجر",
                "descriptionEn": "Store description",
                "feature1TitleAr": "ميزة 1",
                "feature1TitleEn": "Feature 1"
            }
            """;

            var result = AboutHelper.Deserialize(json);
            result.TitleAr.Should().Be("عن المتجر");
            result.TitleEn.Should().Be("About Store");
            result.DescriptionAr.Should().Be("وصف المتجر");
            result.DescriptionEn.Should().Be("Store description");
            result.Feature1TitleAr.Should().Be("ميزة 1");
            result.Feature1TitleEn.Should().Be("Feature 1");
        }

        [Fact]
        public void Deserialize_WithLegacyFormat_ShouldMigrate()
        {
            var json = """
            {
                "Title": "Legacy Title",
                "Description": "Legacy Description",
                "Feature1Title": "F1",
                "Feature1Desc": "FD1",
                "Feature2Title": "F2",
                "Feature2Desc": "FD2",
                "Feature3Title": "F3",
                "Feature3Desc": "FD3"
            }
            """;

            var result = AboutHelper.Deserialize(json);
            result.TitleAr.Should().Be("Legacy Title");
            result.TitleEn.Should().Be("Legacy Title");
            result.DescriptionAr.Should().Be("Legacy Description");
            result.DescriptionEn.Should().Be("Legacy Description");
            result.Feature1TitleAr.Should().Be("F1");
            result.Feature1TitleEn.Should().Be("F1");
            result.Feature1DescAr.Should().Be("FD1");
            result.Feature1DescEn.Should().Be("FD1");
            result.Feature2TitleAr.Should().Be("F2");
            result.Feature3TitleAr.Should().Be("F3");
        }

        [Fact]
        public void Deserialize_WithLegacyFormatLowercase_ShouldMigrate()
        {
            var json = """
            {
                "title": "Lowercase Legacy",
                "description": "Desc",
                "feature1Title": "F1"
            }
            """;

            var result = AboutHelper.Deserialize(json);
            result.TitleAr.Should().Be("Lowercase Legacy");
            result.TitleEn.Should().Be("Lowercase Legacy");
            result.DescriptionAr.Should().Be("Desc");
            result.DescriptionEn.Should().Be("Desc");
        }

        [Fact]
        public void Deserialize_WithInvalidJson_ShouldReturnDefault()
        {
            var result = AboutHelper.Deserialize("{invalid json}");
            result.Should().NotBeNull();
            result.TitleAr.Should().BeEmpty();
        }

        [Fact]
        public void Deserialize_WithPartialData_ShouldFillMissingWithEmpty()
        {
            var json = """{ "titleAr": "Only Title" }""";
            var result = AboutHelper.Deserialize(json);
            result.TitleAr.Should().Be("Only Title");
            result.DescriptionAr.Should().BeEmpty();
            result.Feature1TitleAr.Should().BeEmpty();
        }
    }
}
