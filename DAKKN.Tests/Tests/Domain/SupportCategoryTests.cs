using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportCategoryTests
{
    [Fact]
    public void Create_ShouldInitializeCategory()
    {
        var category = SupportCategory.Create("Technical", "تقني", "Technical support category", "icon-tech", 1);

        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be("Technical");
        category.ArName.Should().Be("تقني");
        category.Description.Should().Be("Technical support category");
        category.Icon.Should().Be("icon-tech");
        category.DisplayOrder.Should().Be(1);
        category.IsActive.Should().BeTrue();
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldUseDefaults_WhenOptionalParamsOmitted()
    {
        var category = SupportCategory.Create("General", "عام");
        category.Description.Should().BeNull();
        category.Icon.Should().BeNull();
        category.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void DefaultIsActive_ShouldBeTrue()
    {
        var category = new SupportCategory();
        category.IsActive.Should().BeTrue();
    }
}
