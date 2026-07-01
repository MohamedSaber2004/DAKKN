using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class EnumTests
{
    [Fact]
    public void Gender_ShouldHaveExpectedValues()
    {
        ((int)Gender.Male).Should().Be(1);
        ((int)Gender.Female).Should().Be(2);
    }

    [Fact]
    public void InventoryTransactionType_ShouldHaveExpectedValues()
    {
        Enum.GetName(InventoryTransactionType.StockAdded).Should().Be("StockAdded");
        Enum.GetName(InventoryTransactionType.StockRemoved).Should().Be("StockRemoved");
        Enum.GetName(InventoryTransactionType.OrderPlaced).Should().Be("OrderPlaced");
        Enum.GetName(InventoryTransactionType.OrderCancelled).Should().Be("OrderCancelled");
        Enum.GetName(InventoryTransactionType.ManualAdjustment).Should().Be("ManualAdjustment");
    }

    [Fact]
    public void LanguageCode_ShouldHaveExpectedValues()
    {
        ((int)LanguageCode.ar).Should().Be(1);
        ((int)LanguageCode.en).Should().Be(2);
    }

    [Fact]
    public void OrderStatus_ShouldHaveAllNineValues()
    {
        ((int)OrderStatus.Pending).Should().Be(1);
        ((int)OrderStatus.Confirmed).Should().Be(2);
        ((int)OrderStatus.Processing).Should().Be(3);
        ((int)OrderStatus.Packed).Should().Be(4);
        ((int)OrderStatus.Shipped).Should().Be(5);
        ((int)OrderStatus.OutForDelivery).Should().Be(6);
        ((int)OrderStatus.Delivered).Should().Be(7);
        ((int)OrderStatus.Cancelled).Should().Be(8);
        ((int)OrderStatus.Refunded).Should().Be(9);

        var values = Enum.GetValues<OrderStatus>();
        values.Should().HaveCount(9);
    }

    [Fact]
    public void ProductStockStatus_ShouldHaveExpectedValues()
    {
        Enum.GetName(ProductStockStatus.InStock).Should().Be("InStock");
        Enum.GetName(ProductStockStatus.LowStock).Should().Be("LowStock");
        Enum.GetName(ProductStockStatus.OutOfStock).Should().Be("OutOfStock");
    }

    [Fact]
    public void SuggestionStatus_ShouldHaveExpectedValues()
    {
        ((int)SuggestionStatus.Pending).Should().Be(1);
        ((int)SuggestionStatus.UnderReview).Should().Be(2);
        ((int)SuggestionStatus.Approved).Should().Be(3);
        ((int)SuggestionStatus.Rejected).Should().Be(4);
    }

    [Fact]
    public void SupportTicketPriority_ShouldHaveExpectedValues()
    {
        ((int)SupportTicketPriority.Low).Should().Be(1);
        ((int)SupportTicketPriority.Medium).Should().Be(2);
        ((int)SupportTicketPriority.High).Should().Be(3);
        ((int)SupportTicketPriority.Urgent).Should().Be(4);
    }

    [Fact]
    public void SupportTicketStatus_ShouldHaveExpectedValues()
    {
        ((int)SupportTicketStatus.Open).Should().Be(1);
        ((int)SupportTicketStatus.InProgress).Should().Be(2);
        ((int)SupportTicketStatus.WaitingCustomer).Should().Be(3);
        ((int)SupportTicketStatus.WaitingStaff).Should().Be(4);
        ((int)SupportTicketStatus.Resolved).Should().Be(5);
        ((int)SupportTicketStatus.Closed).Should().Be(6);
    }

    [Fact]
    public void UserType_ShouldHaveExpectedValues()
    {
        Enum.GetName(UserType.User).Should().Be("User");
        Enum.GetName(UserType.Admin).Should().Be("Admin");
    }
}
