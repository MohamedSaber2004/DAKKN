using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Localization;
using DAKKN.MVC.Models;
using DAKKN.MVC.ViewModels.Admin;
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

        [OutputCache(NoStore = true)]
        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Customer");
            }

            var featuredProducts = await _mediator.Send(new GetFeaturedProductsQuery());
            var categories = await _mediator.Send(new GetCategoriesQuery());
            var allProducts = await _mediator.Send(new GetProductsQuery(null, null, 1, 8));
            var cmsSettings = await _mediator.Send(new GetLandingPageSettingsQuery());

            var hasCmsData = cmsSettings.Hero != "{}" && !string.IsNullOrEmpty(cmsSettings.Hero);

            var sectionOrder = DeserializeOrDefault<SectionOrderViewModel>(cmsSettings.SectionOrder);
            var visibleSections = sectionOrder.Sections
                .Where(s => s.IsVisible)
                .Select(s => s.Id)
                .ToHashSet();
            var orderedSectionIds = sectionOrder.Sections
                .OrderBy(s => s.DisplayOrder)
                .Where(s => s.IsVisible)
                .Select(s => s.Id)
                .ToList();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var viewModel = new LandingPageViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                AllProducts = allProducts,
                Hero = DeserializeOrDefault<HeroSettingsViewModel>(cmsSettings.Hero),
                About = DeserializeOrDefault<AboutSettingsViewModel>(cmsSettings.About),
                Testimonials = SortTopRatedTestimonials(DeserializeOrDefault<TestimonialsSettingsViewModel>(cmsSettings.Testimonials)),
                Contact = DeserializeOrDefault<ContactSettingsViewModel>(cmsSettings.Contact),
                HasCmsData = hasCmsData,
                VisibleSections = visibleSections,
                OrderedSectionIds = orderedSectionIds,
                BaseUrl = baseUrl,
            };

            return View(viewModel);
        }

        private static TestimonialsSettingsViewModel SortTopRatedTestimonials(TestimonialsSettingsViewModel testimonials)
        {
            testimonials.Reviews = testimonials.Reviews
                .OrderByDescending(r => r.Rating)
                .Take(3)
                .ToList();
            return testimonials;
        }

        private static T DeserializeOrDefault<T>(string json) where T : new()
        {
            if (string.IsNullOrEmpty(json) || json == "{}" || json == "[]")
                return new T();
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? new T();
            }
            catch
            {
                return new T();
            }
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
