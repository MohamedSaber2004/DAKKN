using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.Application.Features.Orders.Queries.GetDashboardStats;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews;
using DAKKN.Application.Localization;
using DAKKN.MVC.Helpers;
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
            var brandReviews = await _mediator.Send(new GetDisplayedBrandReviewsQuery());

            var hasCmsData = IsSectionHasData(cmsSettings.Hero)
                || IsSectionHasData(cmsSettings.About)
                || IsSectionHasData(cmsSettings.Testimonials)
                || IsSectionHasData(cmsSettings.Contact);

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

            var stats = await _mediator.Send(new GetDashboardStatsQuery());

            var viewModel = new LandingPageViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                AllProducts = allProducts,
                Hero = HeroHelper.Deserialize(cmsSettings.Hero),
                About = AboutHelper.Deserialize(cmsSettings.About),
                Testimonials = MergeTestimonials(DeserializeOrDefault<TestimonialsSettingsViewModel>(cmsSettings.Testimonials), brandReviews),
                Contact = DeserializeOrDefault<ContactSettingsViewModel>(cmsSettings.Contact),
                HasCmsData = hasCmsData,
                VisibleSections = visibleSections,
                OrderedSectionIds = orderedSectionIds,
                BaseUrl = baseUrl,
                TotalProducts = stats.TotalProducts,
                TotalCustomers = stats.TotalUsers,
                TotalCategories = categories.Count,
                TotalOrders = stats.OrdersToday + stats.PendingOrders + stats.ConfirmedOrders + stats.ProcessingOrders + stats.ShippedOrders + stats.DeliveredOrders + stats.CancelledOrders,
            };

            return View(viewModel);
        }

        private static TestimonialsSettingsViewModel MergeTestimonials(TestimonialsSettingsViewModel cms, List<BrandReviewDto> brandReviews)
        {
            if (brandReviews.Count > 0)
            {
                cms.Reviews = brandReviews.Select(r => new TestimonialItem
                {
                    Name = r.CustomerName,
                    Role = string.Empty,
                    Quote = r.ReviewText,
                    Rating = r.Rating,
                    IsFeatured = true,
                    ProfilePictureUrl = r.ProfilePictureUrl
                }).ToList();
            }
            else
            {
                cms.Reviews = cms.Reviews.OrderByDescending(r => r.Rating).ToList();
            }
            return cms;
        }

        private static bool IsSectionHasData(string json)
        {
            return json != "{}" && !string.IsNullOrEmpty(json);
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
