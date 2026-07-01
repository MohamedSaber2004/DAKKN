using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportFAQCategoryTests
{
    [Fact]
    public void Create_ShouldInitializeCategory()
    {
        var category = SupportFAQCategory.Create("Billing", "الفواتير", "icon-billing", 2);

        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be("Billing");
        category.ArName.Should().Be("الفواتير");
        category.Icon.Should().Be("icon-billing");
        category.DisplayOrder.Should().Be(2);
        category.IsActive.Should().BeTrue();
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldUseDefaults_WhenOptionalParamsOmitted()
    {
        var category = SupportFAQCategory.Create("General", "عام");
        category.Icon.Should().BeNull();
        category.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void DefaultIsActive_ShouldBeTrue()
    {
        var category = new SupportFAQCategory();
        category.IsActive.Should().BeTrue();
    }
}
