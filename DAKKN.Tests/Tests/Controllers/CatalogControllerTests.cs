using DAKKN.Appearence.Controllers.APIs.V1;
using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetPriceRange;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Tests.Tests.Controllers
{
    public class CatalogControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CatalogController _controller;

        public CatalogControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CatalogController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetProducts_ShouldReturnOkWithPaginatedResult()
        {
            var expected = PagginatedResult<ProductDto>.Create(new List<ProductDto>(), 0);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetProducts(null, null, 1, 10, null);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<PagginatedResult<ProductDto>>>().Subject;
            apiResponse.Success.Should().BeTrue();
            apiResponse.Data.Should().Be(expected);
        }

        [Fact]
        public async Task GetProducts_ShouldSendCorrectQuery()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(PagginatedResult<ProductDto>.Create(new List<ProductDto>(), 0, 2, 20));

            await _controller.GetProducts("search", Guid.NewGuid(), 2, 20, 500);

            _mediatorMock.Verify(m => m.Send(
                It.Is<GetProductsQuery>(q =>
                    q.SearchTerm == "search" &&
                    q.PageNumber == 2 &&
                    q.PageSize == 20 &&
                    q.MaxPrice == 500),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnOk()
        {
            var expected = new List<CategoryDto>
            {
                new() { CategoryName = "Cat1", ArName = "تصنيف١" }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetCategories();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<List<CategoryDto>>>().Subject;
            apiResponse.Data.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetFeaturedProducts_ShouldReturnOk()
        {
            var expected = new List<ProductDto>();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetFeaturedProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetFeaturedProducts();

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetPriceRange_ShouldReturnOk()
        {
            var expected = new PriceRangeDto { MinPrice = 10, MaxPrice = 1000 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPriceRangeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetPriceRange();

            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
