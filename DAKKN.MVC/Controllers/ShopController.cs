using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Features.Cart.Queries.GetCart;
using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Orders.Commands.PlaceOrder;
using DAKKN.Application.Features.Orders.Queries.GetOrderDetails;
using DAKKN.Application.Features.Products.Queries.GetProductById;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.Products.Queries.GetRelatedProducts;
using DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates;
using DAKKN.Application.Localization;
using DAKKN.MVC.ViewModels.Customer;
using DAKKN.MVC.ViewModels.Landing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Localization;

namespace DAKKN.MVC.Controllers
{
    [AllowAnonymous]
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
            return View("~/Views/Shop/Products.cshtml", viewModel);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> ProductDetails(Guid id)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                var related = await _mediator.Send(new GetRelatedProductsQuery(id, product.CategoryId));
                ViewData["Title"] = product.Name;
                return View("ProductDetails", new ProductDetailsViewModel { Product = product, RelatedProducts = related });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("cart")]
        [OutputCache(NoStore = true)]
        public async Task<IActionResult> Cart()
        {
            ViewData["Title"] = _localizer["nav_cart"];
            var cart = await _mediator.Send(new GetCartQuery());
            var governorates = await _mediator.Send(new GetActiveShippingGovernoratesQuery());
            var viewModel = new GuestCartViewModel
            {
                Items = cart.Items,
                ShippingGovernorateId = cart.ShippingGovernorateId,
                GovernorateName = cart.GovernorateName,
                GovernorateArName = cart.GovernorateArName,
                ShippingPrice = cart.ShippingPrice,
                Governorates = governorates
            };
            return View(viewModel);
        }

        [HttpGet("checkout")]
        [OutputCache(NoStore = true)]
        public async Task<IActionResult> Checkout()
        {
            ViewData["Title"] = _localizer["checkout_h1"];
            var cart = await _mediator.Send(new GetCartQuery());
            var governorates = await _mediator.Send(new GetActiveShippingGovernoratesQuery());
            var viewModel = new GuestCartViewModel
            {
                Items = cart.Items,
                ShippingGovernorateId = cart.ShippingGovernorateId,
                GovernorateName = cart.GovernorateName,
                GovernorateArName = cart.GovernorateArName,
                ShippingPrice = cart.ShippingPrice,
                Governorates = governorates
            };
            return View(viewModel);
        }

        [HttpPost("place-order")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder([FromForm] PlaceOrderRequest request)
        {
            try
            {
                var command = new PlaceOrderCommand(
                    request.CustomerName,
                    request.CustomerPhone,
                    request.ShippingAddress,
                    request.ShippingGovernorateId,
                    request.Notes);

                var result = await _mediator.Send(command);
                TempData["SuccessMessage"] = _localizer[LocalizationKeys.OrderMessages.Created.Value].Value;
                return RedirectToAction(nameof(OrderConfirmation), new { orderId = result.OrderId });
            }
            catch (BadRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Cart));
            }
            catch (ValidationException ex)
            {
                foreach (var kvp in ex.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        ModelState.AddModelError(kvp.Key, error);
                    }
                }
                return RedirectToAction(nameof(Checkout));
            }
        }

        [HttpGet("order-confirmation/{orderId}")]
        [OutputCache(NoStore = true)]
        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            ViewData["Title"] = _localizer["order_placed_title"];

            try
            {
                var order = await _mediator.Send(new GetOrderDetailsQuery(orderId, IsAdmin: false));
                return View("OrderConfirmation", order);
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
