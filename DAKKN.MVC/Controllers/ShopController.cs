using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Localization;
using DAKKN.MVC.ViewModels.Customer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DAKKN.MVC.Controllers
{
    [Route("shop")]
    public class ShopController : Controller
    {
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IMediator _mediator;

        public ShopController(IStringLocalizer<Messages> localizer, IMediator mediator)
        {
            _localizer = localizer;
            _mediator = mediator;
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products(Guid? categoryId, int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = _localizer["nav_shop"];
            var productsResult = await _mediator.Send(new GetProductsQuery(null, categoryId, pageNumber, pageSize));
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var viewModel = new ProductsViewModel
            {
                Products = productsResult.Items.ToList(),
                Categories = categories,
                PageNumber = productsResult.PageNumber,
                TotalPages = productsResult.TotalPages,
                HasPreviousPage = productsResult.HasPreviousPage,
                HasNextPage = productsResult.HasNextPage
            };
            return View("~/Views/Customer/Products.cshtml", viewModel);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                ViewData["Title"] = product.Name;
                return View("~/Views/Customer/ProductDetails.cshtml", new ProductDetailsViewModel { Product = product });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> Categories()
        {
            ViewData["Title"] = _localizer["nav_categories"];
            var categories = await _mediator.Send(new GetCategoriesQuery());
            return View(categories);
        }
    }
}
