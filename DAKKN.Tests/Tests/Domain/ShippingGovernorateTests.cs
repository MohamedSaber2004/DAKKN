using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class ShippingGovernorateTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var gov = new ShippingGovernorate
        {
            Name = "Cairo",
            ArName = "القاهرة",
            ShippingPrice = 50m,
            DisplayOrder = 1
        };

        gov.Id.Should().NotBeEmpty();
        gov.Name.Should().Be("Cairo");
        gov.ArName.Should().Be("القاهرة");
        gov.ShippingPrice.Should().Be(50m);
        gov.DisplayOrder.Should().Be(1);
    }

    [Fact]
    public void ShippingPrice_ShouldAllowZero()
    {
        var gov = new ShippingGovernorate { ShippingPrice = 0m };
        gov.ShippingPrice.Should().Be(0m);
    }

    [Fact]
    public void DefaultValues_ShouldBeEmptyStrings()
    {
        var gov = new ShippingGovernorate();
        gov.Name.Should().BeEmpty();
        gov.ArName.Should().BeEmpty();
        gov.ShippingPrice.Should().Be(0m);
        gov.DisplayOrder.Should().Be(0);
    }
}
