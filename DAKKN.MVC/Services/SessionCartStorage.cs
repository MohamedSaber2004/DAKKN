using System.Text.Json;
using DAKKN.Application.Features.Cart.DTOs;
using DAKKN.Application.Interfaces;

namespace DAKKN.MVC.Services
{
    public class SessionCartStorage : IGuestCartStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SessionCartStorage> _logger;
        private const string SessionKey = "GuestCart";
        private const string ShippingGovKey = "GuestCart_ShippingGovId";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public SessionCartStorage(IHttpContextAccessor httpContextAccessor, ILogger<SessionCartStorage> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public List<CartItemDto> GetCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                _logger.LogWarning("SessionCartStorage.GetCart: HttpContext.Session is null");
                return new List<CartItemDto>();
            }

            var data = session.GetString(SessionKey);
            if (string.IsNullOrEmpty(data))
                return new List<CartItemDto>();

            try
            {
                return JsonSerializer.Deserialize<List<CartItemDto>>(data, JsonOptions) ?? new List<CartItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SessionCartStorage.GetCart: Failed to deserialize cart data. Key={SessionKey}, DataLength={Length}", SessionKey, data.Length);
                session.Remove(SessionKey);
                return new List<CartItemDto>();
            }
        }

        public void SetCart(List<CartItemDto> items)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                _logger.LogWarning("SessionCartStorage.SetCart: HttpContext.Session is null");
                return;
            }

            var data = JsonSerializer.Serialize(items, JsonOptions);
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
