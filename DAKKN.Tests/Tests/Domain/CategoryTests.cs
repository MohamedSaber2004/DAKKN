using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class CategoryTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var category = new Category
        {
            CategoryName = "Electronics",
            ArName = "إلكترونيات",
            ImageUrl = "http://img.com/cat.png"
        };

        category.Id.Should().NotBeEmpty();
        category.CategoryName.Should().Be("Electronics");
        category.ArName.Should().Be("إلكترونيات");
        category.ImageUrl.Should().Be("http://img.com/cat.png");
        category.Products.Should().BeEmpty();
    }

    [Fact]
    public void DefaultValues_ShouldBeEmptyStrings()
    {
        var category = new Category();
        category.CategoryName.Should().BeEmpty();
        category.ArName.Should().BeEmpty();
        category.ImageUrl.Should().BeNull();
        category.Products.Should().BeEmpty();
    }

    [Fact]
    public void Products_ShouldBeMutable()
    {
        var category = new Category();
        var product = new Product { Name = "Test" };
        category.Products.Add(product);

        category.Products.Should().ContainSingle();
        category.Products.Should().Contain(product);
    }
}
