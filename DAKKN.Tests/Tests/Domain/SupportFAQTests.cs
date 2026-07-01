using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SupportFAQTests
{
    [Fact]
    public void Create_ShouldInitializeBilingualFields()
    {
        var categoryId = Guid.NewGuid();

        var faq = SupportFAQ.Create("How to reset password?", "كيفية إعادة تعيين كلمة المرور؟",
            "Go to settings and click reset.", "انتقل إلى الإعدادات وانقر على إعادة التعيين",
            categoryId, 1);

        faq.Id.Should().NotBeEmpty();
        faq.Question.Should().Be("How to reset password?");
        faq.ArQuestion.Should().Be("كيفية إعادة تعيين كلمة المرور؟");
        faq.Answer.Should().Be("Go to settings and click reset.");
        faq.ArAnswer.Should().Be("انتقل إلى الإعدادات وانقر على إعادة التعيين");
        faq.CategoryId.Should().Be(categoryId);
        faq.DisplayOrder.Should().Be(1);
        faq.IsPublished.Should().BeTrue();
        faq.IsActive.Should().BeTrue();
        faq.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldDefaultDisplayOrderToZero()
    {
        var faq = SupportFAQ.Create("Q?", "س؟", "A", "ج", Guid.NewGuid());
        faq.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void DefaultIsPublished_ShouldBeTrue()
    {
        var faq = new SupportFAQ();
        faq.IsPublished.Should().BeTrue();
    }
}
