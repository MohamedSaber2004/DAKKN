using System.Net;
using DAKKN.Tests.Tests.Performance;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DAKKN.Tests.Tests.Security
{
    public class AuthorizationSecurityTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthorizationSecurityTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true,
                BaseAddress = new Uri("http://localhost")
            });
        }

        [Fact]
        public async Task Anonymous_AdminRoutes_ShouldNotBeAccessible()
        {
            var adminRoutes = new[]
            {
                "/admin",
                "/admin/dashboard",
                "/admin/users",
                "/admin/orders",
                "/admin/inventory",
                "/admin/content",
                "/admin/brand-reviews",
                "/admin/categories",
                "/admin/shipping",
                "/admin/support/dashboard",
                "/admin/support/tickets",
                "/admin/support/faqs",
                "/AdminStickerSuggestion/Index"
            };

            foreach (var route in adminRoutes)
            {
                var response = await _client.GetAsync(route);
                Assert.True(
                    response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized,
                    $"Admin route {route} returned {(int)response.StatusCode} instead of a redirect/forbid for anonymous user");
            }
        }

        [Fact]
        public async Task Anonymous_CustomerRoutes_ShouldNotBeAccessible()
        {
            var customerRoutes = new[]
            {
                "/customer",
                "/customer/products",
                "/customer/favorites",
                "/customer/orders",
                "/customer/cart",
                "/customer/checkout",
                "/customer/profile",
                "/customer/brand-reviews",
                "/customer/support"
            };

            foreach (var route in customerRoutes)
            {
                var response = await _client.GetAsync(route);
                Assert.True(
                    response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized,
                    $"Customer route {route} returned {(int)response.StatusCode} instead of a redirect/forbid for anonymous user");
            }
        }

        [Fact]
        public async Task UserRole_AdminRoutes_ShouldNotBeAccessible()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/admin");
            request.Headers.Add("Cookie", "TestAuthRole=User");
            var response = await _client.SendAsync(request);

            Assert.True(
                response.StatusCode == HttpStatusCode.Redirect ||
                response.StatusCode == HttpStatusCode.Forbidden,
                $"Admin route /admin returned {(int)response.StatusCode} instead of a redirect/forbid for User role");
        }

        [Fact]
        public async Task Anonymous_PublicRoutes_ShouldBeAccessible()
        {
            var publicRoutes = new[]
            {
                "/",
                "/Home/Privacy",
                "/Home/Terms",
                "/Home/About",
                "/auth/login",
                "/auth/register",
                "/auth/forgot-password",
                "/shop/products",
                "/shop/categories"
            };

            foreach (var route in publicRoutes)
            {
                var response = await _client.GetAsync(route);
                Assert.True(
                    response.StatusCode == HttpStatusCode.OK ||
                    response.StatusCode == HttpStatusCode.Redirect,
                    $"Public route {route} returned {(int)response.StatusCode} for anonymous user (expected 200 or redirect)");
            }
        }

        [Fact]
        public async Task AdminRole_AdminRoutes_ShouldBeAccessible()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/admin/dashboard");
            request.Headers.Add("Cookie", "TestAuthRole=Admin");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UserRole_CustomerRoutes_ShouldBeAccessible()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/customer/products");
            request.Headers.Add("Cookie", "TestAuthRole=User");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AdminRole_CustomerRoutes_ShouldBeAccessible()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/customer/products");
            request.Headers.Add("Cookie", "TestAuthRole=Admin");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Anonymous_StickerSuggestionRoutes_ShouldNotBeAccessible()
        {
            var routes = new[]
            {
                "/StickerSuggestion/Submit",
                "/StickerSuggestion/MySuggestions"
            };

            foreach (var route in routes)
            {
                var response = await _client.GetAsync(route);
                Assert.True(
                    response.StatusCode == HttpStatusCode.Redirect ||
                    response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized,
                    $"StickerSuggestion route {route} returned {(int)response.StatusCode} instead of redirect/forbid for anonymous user");
            }
        }

        [Fact]
        public async Task UserRole_StickerSuggestionRoutes_ShouldBeAccessible()
        {
            var routes = new[]
            {
                "/StickerSuggestion/Submit",
                "/StickerSuggestion/MySuggestions"
            };

            foreach (var route in routes)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, route);
                request.Headers.Add("Cookie", "TestAuthRole=User");
                var response = await _client.SendAsync(request);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
