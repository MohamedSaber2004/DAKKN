using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.MVC.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DAKKN.Tests.Tests.Controllers
{
    public class SessionCartStorageTests
    {
        private SessionCartStorage CreateStorage(HttpContext? httpContext = null)
        {
            httpContext ??= new DefaultHttpContext();
            var session = new TestSession();
            httpContext.Session = session;

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            return new SessionCartStorage(accessorMock.Object);
        }

        [Fact]
        public void GetCart_WhenEmpty_ShouldReturnEmptyList()
        {
            var storage = CreateStorage();
            var cart = storage.GetCart();
            cart.Should().BeEmpty();
        }

        [Fact]
        public void SetCart_ShouldStoreItems()
        {
            var storage = CreateStorage();
            var items = new List<CartItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 2 }
            };

            storage.SetCart(items);
            var retrieved = storage.GetCart();

            retrieved.Should().HaveCount(1);
            retrieved[0].Name.Should().Be("Test");
            retrieved[0].Quantity.Should().Be(2);
        }

        [Fact]
        public void GetCartCount_ShouldReturnTotalQuantity()
        {
            var storage = CreateStorage();
            var items = new List<CartItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Name = "A", Price = 10, Quantity = 3 },
                new() { ProductId = Guid.NewGuid(), Name = "B", Price = 20, Quantity = 5 }
            };
            storage.SetCart(items);

            var count = storage.GetCartCount();
            count.Should().Be(8);
        }

        [Fact]
        public void ClearCart_ShouldRemoveAllItems()
        {
            var storage = CreateStorage();
            storage.SetCart(new List<CartItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 1 }
            });

            storage.ClearCart();
            var retrieved = storage.GetCart();
            retrieved.Should().BeEmpty();
        }

        [Fact]
        public void GetShippingGovernorateId_WhenSet_ShouldReturnValue()
        {
            var storage = CreateStorage();
            var govId = Guid.NewGuid();
            storage.SetShippingGovernorateId(govId);

            var retrieved = storage.GetShippingGovernorateId();
            retrieved.Should().Be(govId);
        }

        [Fact]
        public void GetShippingGovernorateId_WhenNotSet_ShouldReturnNull()
        {
            var storage = CreateStorage();
            var result = storage.GetShippingGovernorateId();
            result.Should().BeNull();
        }

        [Fact]
        public void SetShippingGovernorateId_WithNull_ShouldClear()
        {
            var storage = CreateStorage();
            storage.SetShippingGovernorateId(Guid.NewGuid());
            storage.SetShippingGovernorateId(null);

            var result = storage.GetShippingGovernorateId();
            result.Should().BeNull();
        }

        [Fact]
        public void GetCart_WhenNoHttpContext_ShouldReturnEmptyList()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var storage = new SessionCartStorage(accessorMock.Object);

            var cart = storage.GetCart();
            cart.Should().BeEmpty();
        }

        [Fact]
        public void SetCart_WhenNoHttpContext_ShouldNotThrow()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var storage = new SessionCartStorage(accessorMock.Object);

            var act = () => storage.SetCart(new List<CartItemDto>
            {
                new() { ProductId = Guid.NewGuid(), Name = "Test", Price = 10, Quantity = 1 }
            });
            act.Should().NotThrow();
        }

        [Fact]
        public void GetCart_WithCorruptData_ShouldReturnEmptyList()
        {
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("GuestCart", "not-valid-json");
            httpContext.Session = session;

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            var storage = new SessionCartStorage(accessorMock.Object);

            var cart = storage.GetCart();
            cart.Should().BeEmpty();
        }
    }

    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new(StringComparer.OrdinalIgnoreCase);

        public string Id => "test-session-id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken ct = default) => Task.CompletedTask;
        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
    }

    public static class TestSessionExtensions
    {
        public static void SetString(this ISession session, string key, string value)
            => session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));

        public static string? GetString(this ISession session, string key)
        {
            if (session.TryGetValue(key, out var data))
                return System.Text.Encoding.UTF8.GetString(data);
            return null;
        }
    }
}
