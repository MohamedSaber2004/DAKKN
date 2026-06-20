using System.Text.Json;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;

namespace DAKKN.MVC.Services
{
    public class SessionCartStorage : IGuestCartStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "GuestCart";

        public SessionCartStorage(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItemDto> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<CartItemDto>();

            var data = session.GetString(SessionKey);
            if (string.IsNullOrEmpty(data))
                return new List<CartItemDto>();

            try
            {
                return JsonSerializer.Deserialize<List<CartItemDto>>(data) ?? new List<CartItemDto>();
            }
            catch
            {
                return new List<CartItemDto>();
            }
        }

        public void SetCart(List<CartItemDto> items)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var data = JsonSerializer.Serialize(items);
            session.SetString(SessionKey, data);
        }

        public int GetCartCount()
        {
            return GetCart().Sum(x => x.Quantity);
        }

        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(SessionKey);
        }
    }
}
