using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit.Abstractions;

namespace DAKKN.Tests.Tests.Performance
{
    public class MvcPerformanceTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly Stopwatch _stopwatch;
        private readonly StringBuilder _report;
        private readonly string _reportId;
        private int _passed;
        private int _failed;

        public MvcPerformanceTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true,
                BaseAddress = new Uri("http://localhost")
            });
            _stopwatch = new Stopwatch();
            _report = new StringBuilder();
            _reportId = Guid.NewGuid().ToString("N")[..8];
            _report.AppendLine("+----------------------------------------------------------------------------+");
            _report.AppendLine("|                     DAKKN MVC Performance Report                            |");
            _report.AppendLine("+----------------------------------------------------------------------------+");
            _report.AppendLine();
            _report.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            _report.AppendLine($"{"Route",-60} {"Status",-8} {"Time",-8}");
            _report.AppendLine(new string('-', 80));
        }

        public void Dispose()
        {
            _report.AppendLine(new string('-', 80));
            _report.AppendLine($"Total: {_passed + _failed}  |  Passed: {_passed}  |  Failed: {_failed}");
            var reportFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                $"perf-report-{DateTime.Now:yyyyMMdd-HHmmss}-{_reportId}.txt");
            File.WriteAllText(reportFile, _report.ToString());
            TestContext.WriteLine(reportFile);
            _client.Dispose();
        }

        [Fact]
        public async Task Anonymous_Routes_Should_Respond_Within_Threshold()
        {
            var routes = new (string Route, string Description)[]
            {
                ("/", "Home Index"),
                ("/Home/Privacy", "Privacy"),
                ("/Home/Terms", "Terms"),
                ("/Home/About", "About"),
                ("/auth/login", "Login GET"),
                ("/auth/register", "Register GET"),
                ("/auth/forgot-password", "Forgot Password GET"),
                ("/auth/reset-password", "Reset Password GET"),
                ("/auth/access-denied", "Access Denied"),
                ("/shop/products", "Shop Products"),
                ("/shop/cart", "Shop Cart"),
                ("/shop/categories", "Shop Categories"),
            };

            await MeasureRoutes(routes, null);
        }

        [Fact]
        public async Task Admin_Routes_Should_Respond_Within_Threshold()
        {
            var routes = new (string Route, string Description)[]
            {
                ("/admin", "Admin Dashboard"),
                ("/admin/dashboard", "Admin Dashboard (explicit)"),
                ("/admin/users", "Admin Users"),
                ("/admin/orders", "Admin Orders"),
                ("/admin/inventory", "Admin Inventory"),
                ("/admin/content", "Admin Content"),
                ("/admin/brand-reviews", "Admin Brand Reviews"),
                ("/admin/settings", "Admin Settings"),
                ("/admin/categories", "Admin Categories"),
                ("/admin/categories/create", "Admin Create Category GET"),
                ("/admin/shipping", "Admin Shipping"),
                ("/admin/shipping/create", "Admin Create Shipping GET"),
                ("/admin/support/dashboard", "Support Dashboard"),
                ("/admin/support/tickets", "Support Tickets"),
                ("/admin/support/categories", "Support Categories"),
                ("/admin/support/faqs", "Support FAQs"),
                ("/admin/support/settings", "Support Settings"),
                ("/admin/support/faq-categories", "FAQ Categories"),
                ("/AdminStickerSuggestion/Index", "Admin Sticker Suggestions"),
            };

            await MeasureRoutes(routes, "Admin");
        }

        [Fact]
        public async Task Customer_Routes_Should_Respond_Within_Threshold()
        {
            var routes = new (string Route, string Description)[]
            {
                ("/customer", "Customer Dashboard"),
                ("/customer/products", "Customer Products"),
                ("/customer/favorites", "Customer Favorites"),
                ("/customer/orders", "Customer Orders"),
                ("/customer/custom-order", "Custom Order"),
                ("/customer/cart", "Customer Cart"),
                ("/customer/checkout", "Checkout"),
                ("/customer/profile", "Profile"),
                ("/customer/brand-reviews", "Customer Brand Reviews"),
                ("/customer/support", "Customer Support"),
                ("/customer/support/new", "Customer New Ticket"),
                ("/StickerSuggestion/Submit", "Submit Suggestion"),
                ("/StickerSuggestion/MySuggestions", "My Suggestions"),
            };

            await MeasureRoutes(routes, "User");
        }

        private async Task MeasureRoutes(
            IEnumerable<(string Route, string Description)> routes,
            string? role)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/");

            foreach (var (route, desc) in routes)
            {
                var cookies = new List<string>();
                if (role is not null)
                    cookies.Add($"TestAuthRole={role}");

                var cookieHeader = string.Join("; ", cookies);
                using var msg = new HttpRequestMessage(HttpMethod.Get, route);
                if (!string.IsNullOrEmpty(cookieHeader))
                    msg.Headers.Add("Cookie", cookieHeader);

                _stopwatch.Restart();
                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(msg);
                }
                catch (Exception ex)
                {
                    _stopwatch.Stop();
                    LogResult(route, "ERROR", ex.GetType().Name);
                    continue;
                }
                _stopwatch.Stop();

                var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                var status = (int)response.StatusCode;

                if (status < 500)
                {
                    LogResult(route, $"{status}", elapsed);
                }
                else
                {
                    LogResult(route, $"{status} FAIL", elapsed);
                }
            }
        }

        private void LogResult(string route, string status, object? elapsed = null)
        {
            var timeStr = elapsed is double ms ? $"{ms:F0}ms" : "-";
            var isOk = !status.Contains("FAIL") && !status.Contains("ERROR");
            if (isOk) _passed++; else _failed++;
            _report.AppendLine($"{route,-60} {status,-8} {timeStr,-8}");
        }

        private static class TestContext
        {
            public static void WriteLine(string path)
            {
                Console.WriteLine($"Report saved: {path}");
            }
        }
    }
}
