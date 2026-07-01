using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class InventoryTransactionTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var transaction = new InventoryTransaction
        {
            ProductId = Guid.NewGuid(),
            QuantityChanged = 10,
            PreviousQuantity = 5,
            NewQuantity = 15,
            TransactionType = InventoryTransactionType.StockAdded,
            Notes = "Restock"
        };

        transaction.Id.Should().NotBeEmpty();
        transaction.ProductId.Should().NotBeEmpty();
        transaction.QuantityChanged.Should().Be(10);
        transaction.PreviousQuantity.Should().Be(5);
        transaction.NewQuantity.Should().Be(15);
        transaction.TransactionType.Should().Be(InventoryTransactionType.StockAdded);
        transaction.Notes.Should().Be("Restock");
    }

    [Fact]
    public void TransactionType_ShouldBeStockAdded_WhenProductIsRestocked()
    {
        var transaction = new InventoryTransaction { TransactionType = InventoryTransactionType.StockAdded };
        transaction.TransactionType.Should().Be(InventoryTransactionType.StockAdded);
    }

    [Fact]
    public void TransactionType_ShouldBeStockRemoved_WhenProductIsRemoved()
    {
        var transaction = new InventoryTransaction { TransactionType = InventoryTransactionType.StockRemoved };
        transaction.TransactionType.Should().Be(InventoryTransactionType.StockRemoved);
    }

    [Fact]
    public void TransactionType_ShouldBeOrderPlaced_WhenOrderIsPlaced()
    {
        var transaction = new InventoryTransaction { TransactionType = InventoryTransactionType.OrderPlaced };
        transaction.TransactionType.Should().Be(InventoryTransactionType.OrderPlaced);
    }

    [Fact]
    public void TransactionType_ShouldBeOrderCancelled_WhenOrderIsCancelled()
    {
        var transaction = new InventoryTransaction { TransactionType = InventoryTransactionType.OrderCancelled };
        transaction.TransactionType.Should().Be(InventoryTransactionType.OrderCancelled);
    }

    [Fact]
    public void TransactionType_ShouldBeManualAdjustment_WhenManuallyAdjusted()
    {
        var transaction = new InventoryTransaction { TransactionType = InventoryTransactionType.ManualAdjustment };
        transaction.TransactionType.Should().Be(InventoryTransactionType.ManualAdjustment);
    }

    [Fact]
    public void Notes_ShouldBeOptional()
    {
        var transaction = new InventoryTransaction();
        transaction.Notes.Should().BeNull();
    }

    [Fact]
    public void QuantityChanged_ShouldAllowNegativeValues()
    {
        var transaction = new InventoryTransaction { QuantityChanged = -5 };
        transaction.QuantityChanged.Should().Be(-5);
    }
}
