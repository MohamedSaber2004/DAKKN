using Asp.Versioning;
using DAKKN.Appearence.Controllers.APIs;
using DAKKN.Appearence.Routes;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetPriceRange;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DAKKN.Appearence.Controllers.APIs.V1
{
    [ApiVersion("1.0")]
    public class CatalogController : BaseApiController
    {
        public CatalogController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [Route(ApiRoutes.Catalog.Products)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery] string? searchTerm, [FromQuery] Guid? categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] decimal? maxPrice = null)
        {
            var result = await _mediator.Send(new GetProductsQuery(searchTerm, categoryId, pageNumber, pageSize, maxPrice));
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Catalog.Categories)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new GetCategoriesQuery());
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Catalog.FeaturedProducts)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var result = await _mediator.Send(new GetFeaturedProductsQuery());
            return Ok(result);
        }

        [HttpGet]
        [Route(ApiRoutes.Catalog.PriceRange)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPriceRange()
        {
            var result = await _mediator.Send(new GetPriceRangeQuery());
            return Ok(result);
        }
    }
}
