using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Localization;
using DAKKN.MVC.Models;
using DAKKN.MVC.ViewModels.Landing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace DAKKN.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<Messages> _localizer;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IStringLocalizer<Messages> localizer)
        {
            _logger = logger;
            _mediator = mediator;
            _localizer = localizer;
        }

        [OutputCache(Duration = 600)]
        public async Task<IActionResult> Index()
        {
            // Redirect authorized users to the Customer Dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Customer");
            }

            var featuredProducts = await _mediator.Send(new GetFeaturedProductsQuery());
            var categories = await _mediator.Send(new GetCategoriesQuery(Top: 8));
            var allProducts = await _mediator.Send(new GetProductsQuery(null, null, 1, 8));

            var viewModel = new LandingPageViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                AllProducts = allProducts
            };

            return View(viewModel);
        }

        [OutputCache(Duration = 600)]
        public IActionResult Privacy()
        {
            return View();
        }

        [OutputCache(Duration = 600)]
        public IActionResult Terms()
        {
            return View();
        }

        [OutputCache(Duration = 600)]
        public IActionResult About()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
