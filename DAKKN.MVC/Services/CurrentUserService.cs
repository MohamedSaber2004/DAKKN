using DAKKN.Application.Common.Interfaces;
using System.Security.Claims;

namespace DAKKN.Appearence.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null || user.Identity?.IsAuthenticated != true)
                    return Guid.Empty;

                var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                  user.FindFirst("sub")?.Value ??
                                  user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                                  user.FindFirst("uid")?.Value;

                if (Guid.TryParse(userIdString, out var userId))
                    return userId;

                // Fallback: search all claims for any value that parses as a GUID
                var guidClaim = user.Claims.FirstOrDefault(c => Guid.TryParse(c.Value, out _));
                if (guidClaim != null && Guid.TryParse(guidClaim.Value, out var fallbackId))
                    return fallbackId;

                return Guid.Empty;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
