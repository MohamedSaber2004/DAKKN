using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain
{
    public class ProductTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            var product = new Product
            {
                Name = "Test Product",
                ArName = "منتج تجريبي",
                Price = 100m
            };

            product.Id.Should().NotBeEmpty();
            product.Name.Should().Be("Test Product");
            product.Price.Should().Be(100m);
            product.QuantityInStock.Should().Be(0);
            product.IsInStock.Should().BeFalse();
            product.StockStatus.Should().Be(ProductStockStatus.OutOfStock);
        }

        [Fact]
        public void StockStatus_ShouldBeInStock_WhenQuantityExceedsDanger()
        {
            var product = new Product { QuantityInStock = 10, DangerQuantity = 5 };
            product.StockStatus.Should().Be(ProductStockStatus.InStock);
            product.IsInStock.Should().BeTrue();
        }

        [Fact]
        public void StockStatus_ShouldBeLowStock_WhenQuantityIsAtOrBelowDanger()
        {
            var product = new Product { QuantityInStock = 3, DangerQuantity = 5 };
            product.StockStatus.Should().Be(ProductStockStatus.LowStock);
            product.IsInStock.Should().BeTrue();
        }

        [Fact]
        public void StockStatus_ShouldBeOutOfStock_WhenQuantityIsZero()
        {
            var product = new Product { QuantityInStock = 0, DangerQuantity = 5 };
            product.StockStatus.Should().Be(ProductStockStatus.OutOfStock);
            product.IsInStock.Should().BeFalse();
        }

        [Fact]
        public void ReduceStock_ShouldDecreaseQuantity()
        {
            var product = new Product { QuantityInStock = 10 };
            product.ReduceStock(4);
            product.QuantityInStock.Should().Be(6);
        }

        [Fact]
        public void ReduceStock_ShouldThrow_WhenQuantityExceedsStock()
        {
            var product = new Product { QuantityInStock = 3 };
            Action act = () => product.ReduceStock(10);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Insufficient stock*");
        }

        [Fact]
        public void ReduceStock_ShouldThrow_WhenQuantityIsZeroOrNegative()
        {
            var product = new Product { QuantityInStock = 10 };
            Action act = () => product.ReduceStock(0);
            act.Should().Throw<ArgumentException>();

            act = () => product.ReduceStock(-1);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void IncreaseStock_ShouldAddQuantity()
        {
            var product = new Product { QuantityInStock = 5 };
            product.IncreaseStock(3);
            product.QuantityInStock.Should().Be(8);
        }

        [Fact]
        public void IncreaseStock_ShouldThrow_WhenQuantityIsZeroOrNegative()
        {
            var product = new Product();
            Action act = () => product.IncreaseStock(0);
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetStock_ShouldUpdateValues()
        {
            var product = new Product();
            product.SetStock(20, 3);
            product.QuantityInStock.Should().Be(20);
            product.DangerQuantity.Should().Be(3);
        }

        [Fact]
        public void SetStock_ShouldThrow_WhenQuantityIsNegative()
        {
            var product = new Product();
            Action act = () => product.SetStock(-1, 5);
            act.Should().Throw<ArgumentException>();

            act = () => product.SetStock(10, -1);
            act.Should().Throw<ArgumentException>();
        }
    }
}
