using DAKKN.Application.Common.Models;

namespace DAKKN.Tests.Tests.Application.Models
{
    public class PagginatedResultTests
    {
        [Fact]
        public void Constructor_ShouldCalculateTotalPages()
        {
            var items = new[] { 1, 2, 3, 4, 5 };
            var result = new PagginatedResult<int>(items, 20, 1, 5);

            result.TotalPages.Should().Be(4);
            result.TotalCount.Should().Be(20);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(5);
        }

        [Fact]
        public void HasPreviousPage_ShouldBeFalse_OnFirstPage()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 0, 1, 10);
            result.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public void HasPreviousPage_ShouldBeTrue_OnLaterPage()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 100, 3, 10);
            result.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void HasNextPage_ShouldBeFalse_OnLastPage()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 30, 3, 10);
            result.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public void HasNextPage_ShouldBeTrue_WhenMorePagesExist()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 100, 1, 10);
            result.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void PageNumber_ShouldDefaultToOne_WhenZero()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 10, 0, 10);
            result.PageNumber.Should().Be(1);
        }

        [Fact]
        public void PageSize_ShouldBeCapped_AtMax()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 500, 1, 200);
            result.PageSize.Should().Be(100);
        }

        [Fact]
        public void PageSize_ShouldDefault_WhenLessThanOne()
        {
            var result = new PagginatedResult<int>(Array.Empty<int>(), 10, 1, 0);
            result.PageSize.Should().Be(10);
        }

        [Fact]
        public void Create_ShouldReturnPaginatedResult()
        {
            var items = new[] { "a", "b" };
            var result = PagginatedResult<string>.Create(items, 10, 1, 10);

            result.Items.Should().BeEquivalentTo(items);
            result.TotalCount.Should().Be(10);
        }
    }
}
