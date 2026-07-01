using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldCalculateTotalPrice()
    {
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var item = new OrderItem(orderId, productId, "Widget", "img.jpg", 25.50m, 3);

        item.Id.Should().NotBeEmpty();
        item.OrderId.Should().Be(orderId);
        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be("Widget");
        item.ProductImageUrl.Should().Be("img.jpg");
        item.UnitPrice.Should().Be(25.50m);
        item.Quantity.Should().Be(3);
        item.TotalPrice.Should().Be(76.50m);
    }

    [Fact]
    public void TotalPrice_ShouldBeUnitPriceTimesQuantity()
    {
        var item = new OrderItem(Guid.NewGuid(), Guid.NewGuid(), "Test", null, 10m, 5);
        item.TotalPrice.Should().Be(50m);
    }

    [Fact]
    public void TotalPrice_ShouldBeZero_WhenQuantityIsZero()
    {
        var item = new OrderItem(Guid.NewGuid(), Guid.NewGuid(), "Test", null, 100m, 0);
        item.TotalPrice.Should().Be(0m);
    }

    [Fact]
    public void Constructor_ShouldAllowNullImageUrl()
    {
        var item = new OrderItem(Guid.NewGuid(), Guid.NewGuid(), "Test", null, 10m, 1);
        item.ProductImageUrl.Should().BeNull();
    }
}
