using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class StickerSuggestionTests
{
    [Fact]
    public void Create_ShouldReturnPendingSuggestion()
    {
        var userId = Guid.NewGuid();
        var suggestion = StickerSuggestion.Create(userId, "Cool Sticker", "A really cool sticker design", null, "fun, cool");

        suggestion.Id.Should().NotBeEmpty();
        suggestion.UserId.Should().Be(userId);
        suggestion.Title.Should().Be("Cool Sticker");
        suggestion.Description.Should().Be("A really cool sticker design");
        suggestion.ReferenceImagePath.Should().BeNull();
        suggestion.Tags.Should().Be("fun, cool");
        suggestion.Status.Should().Be(SuggestionStatus.Pending);
    }

    [Fact]
    public void Create_ShouldTrimTitleAndDescription()
    {
        var suggestion = StickerSuggestion.Create(Guid.NewGuid(), "  My Title  ", "  My Desc  ", null, null);
        suggestion.Title.Should().Be("My Title");
        suggestion.Description.Should().Be("My Desc");
    }

    [Fact]
    public void Create_ShouldThrow_WhenTitleIsEmpty()
    {
        Action act = () => StickerSuggestion.Create(Guid.NewGuid(), "", "desc", null, null);
        act.Should().Throw<ArgumentException>().WithMessage("*Title*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenTitleIsWhitespace()
    {
        Action act = () => StickerSuggestion.Create(Guid.NewGuid(), "   ", "desc", null, null);
        act.Should().Throw<ArgumentException>().WithMessage("*Title*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenDescriptionIsEmpty()
    {
        Action act = () => StickerSuggestion.Create(Guid.NewGuid(), "Title", "", null, null);
        act.Should().Throw<ArgumentException>().WithMessage("*Description*");
    }

    [Fact]
    public void MarkUnderReview_ShouldChangeStatus()
    {
        var suggestion = CreatePendingSuggestion();
        var adminId = Guid.NewGuid();

        suggestion.MarkUnderReview(adminId);

        suggestion.Status.Should().Be(SuggestionStatus.UnderReview);
    }

    [Fact]
    public void Approve_ShouldChangeStatusToApproved()
    {
        var suggestion = CreatePendingSuggestion();
        var adminId = Guid.NewGuid();

        suggestion.Approve("Looks good!", adminId);

        suggestion.Status.Should().Be(SuggestionStatus.Approved);
        suggestion.AdminNote.Should().Be("Looks good!");
    }

    [Fact]
    public void Approve_ShouldAllowNullAdminNote()
    {
        var suggestion = CreatePendingSuggestion();
        suggestion.Approve(null, Guid.NewGuid());

        suggestion.AdminNote.Should().BeNull();
    }

    [Fact]
    public void Reject_ShouldChangeStatusToRejected()
    {
        var suggestion = CreatePendingSuggestion();
        var adminId = Guid.NewGuid();

        suggestion.Reject("Not suitable", adminId);

        suggestion.Status.Should().Be(SuggestionStatus.Rejected);
        suggestion.AdminNote.Should().Be("Not suitable");
    }

    [Fact]
    public void LinkProduct_ShouldSetConvertedProductId()
    {
        var suggestion = CreatePendingSuggestion();
        var productId = Guid.NewGuid();

        suggestion.LinkProduct(productId, Guid.NewGuid());

        suggestion.ConvertedProductId.Should().Be(productId);
    }

    private static StickerSuggestion CreatePendingSuggestion()
    {
        return StickerSuggestion.Create(Guid.NewGuid(), "Test", "Test desc", null, null);
    }
}
