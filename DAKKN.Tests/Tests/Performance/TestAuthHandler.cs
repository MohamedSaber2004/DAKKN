using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DAKKN.Tests.Tests.Performance
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Test";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? role = null;

            if (Context.Request.Cookies.TryGetValue("TestAuthRole", out var roleCookie))
                role = roleCookie;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "test-admin-id"),
                new(ClaimTypes.Name, "Test Admin"),
                new("FullName", "Test Admin"),
            };

            if (role is not null)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var identity = new ClaimsIdentity(claims, SchemeName);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, SchemeName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
