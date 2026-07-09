using DAKKN.Application.Features.Categories.Queries.GetCategories;
using DAKKN.Application.Features.CMS.Queries.GetLandingPageSettings;
using DAKKN.Application.Features.Orders.Queries.GetDashboardStats;
using DAKKN.Application.Features.Products.Queries.GetFeaturedProducts;
using DAKKN.Application.Features.Products.Queries.GetProducts;
using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Application.Features.Support.Queries.GetFAQs;
using DAKKN.Application.Localization;
using DAKKN.MVC.Helpers;
using DAKKN.MVC.Models;
using DAKKN.MVC.ViewModels.Admin;
using DAKKN.MVC.ViewModels.Landing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Text;

namespace DAKKN.MVC.Controllers
{
    [AllowAnonymous]
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
                // Route authenticated users to the correct dashboard based on their role
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");

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
                || IsSectionHasData(cmsSettings.Contact)
                || IsSectionHasData(cmsSettings.Faq);

            var sectionOrder = DeserializeOrDefault<SectionOrderViewModel>(cmsSettings.SectionOrder);
            EnsureDefaultSections(sectionOrder);
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

            var faqSettings = DeserializeOrDefault<FaqSettingsViewModel>(cmsSettings.Faq);
            List<SupportFAQDto> faqItems = new();
            if (faqSettings.AutoFetchFromSupport)
            {
                var supportFaqs = await _mediator.Send(new GetFAQsQuery());
                faqItems = supportFaqs.Take(faqSettings.DisplayLimit).ToList();
            }

            var supportCategories = await _mediator.Send(new DAKKN.Application.Features.Support.Queries.GetCategories.GetCategoriesQuery(false));

            var viewModel = new LandingPageViewModel
            {
                FeaturedProducts = featuredProducts,
                Categories = categories,
                AllProducts = allProducts,
                Hero = HeroHelper.Deserialize(cmsSettings.Hero),
                About = AboutHelper.Deserialize(cmsSettings.About),
                Testimonials = MergeTestimonials(DeserializeOrDefault<TestimonialsSettingsViewModel>(cmsSettings.Testimonials), brandReviews),
                Contact = DeserializeOrDefault<ContactSettingsViewModel>(cmsSettings.Contact),
                Faq = faqSettings,
                FaqItems = faqItems,
                SupportCategories = supportCategories,
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

        private static void EnsureDefaultSections(SectionOrderViewModel sectionOrder)
        {
            var defaultSections = new SectionOrderViewModel().Sections;
            foreach (var defaultSection in defaultSections)
            {
                if (!sectionOrder.Sections.Any(s => s.Id == defaultSection.Id))
                {
                    sectionOrder.Sections.Add(defaultSection);
                }
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
        public IActionResult Error(int? statusCode = null)
        {
            var code = statusCode ?? HttpContext.Response.StatusCode;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = code
            });
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [Route("sitemap.xml")]
        public async Task<IActionResult> Sitemap()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var categories = await _mediator.Send(new GetCategoriesQuery(null, false, null));
            var productsResult = await _mediator.Send(new GetProductsQuery(null, null, 1, 5000));

            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xml.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            // Static pages
            var staticUrls = new[]
            {
                new { loc = $"{baseUrl}/",       priority = "1.0", changefreq = "weekly",  lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                new { loc = $"{baseUrl}/shop",    priority = "0.9", changefreq = "daily",   lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                new { loc = $"{baseUrl}/about",   priority = "0.7", changefreq = "monthly", lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                new { loc = $"{baseUrl}/privacy", priority = "0.4", changefreq = "yearly",  lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd") },
                new { loc = $"{baseUrl}/terms",   priority = "0.4", changefreq = "yearly",  lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd") },
            };

            foreach (var url in staticUrls)
            {
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{url.loc}</loc>");
                xml.AppendLine($"    <lastmod>{url.lastmod}</lastmod>");
                xml.AppendLine($"    <changefreq>{url.changefreq}</changefreq>");
                xml.AppendLine($"    <priority>{url.priority}</priority>");
                xml.AppendLine("  </url>");
            }

            // Categories
            foreach (var category in categories.Where(c => !c.IsDeleted))
            {
                var lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd");
                var loc = $"{baseUrl}/shop/products?categoryId={category.Id}";
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{loc}</loc>");
                xml.AppendLine($"    <lastmod>{lastmod}</lastmod>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.8</priority>");
                xml.AppendLine("  </url>");
            }

            // Products
            foreach (var product in productsResult.Items.Where(p => p.IsActive && !p.IsDeleted))
            {
                var lastmod = product.UpdatedAt?.ToString("yyyy-MM-dd") ?? product.CreatedAt.ToString("yyyy-MM-dd");
                var loc = $"{baseUrl}/shop/product/{product.Id}";
                xml.AppendLine("  <url>");
                xml.AppendLine($"    <loc>{loc}</loc>");
                xml.AppendLine($"    <lastmod>{lastmod}</lastmod>");
                xml.AppendLine("    <changefreq>weekly</changefreq>");
                xml.AppendLine("    <priority>0.7</priority>");
                xml.AppendLine("  </url>");
            }

            xml.AppendLine("</urlset>");

            return Content(xml.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}
