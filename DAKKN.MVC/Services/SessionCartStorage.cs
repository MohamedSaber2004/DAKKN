using System.Text.Json;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;

namespace DAKKN.MVC.Services
{
    public class SessionCartStorage : IGuestCartStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string SessionKey = "GuestCart";
        private const string ShippingGovKey = "GuestCart_ShippingGovId";

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
            if (session == null) return;
            session.Remove(SessionKey);
            session.Remove(ShippingGovKey);
        }

        public Guid? GetShippingGovernorateId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return null;

            var data = session.GetString(ShippingGovKey);
            if (string.IsNullOrEmpty(data)) return null;

            if (Guid.TryParse(data, out var id))
                return id;

            return null;
        }

        public void SetShippingGovernorateId(Guid? governorateId)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            if (governorateId.HasValue)
                session.SetString(ShippingGovKey, governorateId.Value.ToString());
            else
                session.Remove(ShippingGovKey);
        }
    }
}
