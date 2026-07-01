using DAKKN.Appearence.Filters;
using DAKKN.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace DAKKN.Tests.Tests.Controllers
{
    public class RoleAuthorizeAttributeTests
    {
        private AuthorizationFilterContext CreateContext(ClaimsPrincipal? user, string path = "/api/test")
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = path;
            if (user != null)
                httpContext.User = user;

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }

        [Fact]
        public void OnAuthorization_WithNoUser_ShouldReturn401ForApi()
        {
            var context = CreateContext(null, "/api/test");
            var attr = new RoleAuthorizeAttribute(UserType.Admin);

            attr.OnAuthorization(context);

            context.Result.Should().NotBeNull();
            var jsonResult = context.Result.As<JsonResult>();
            jsonResult.StatusCode.Should().Be(401);
        }

        [Fact]
        public void OnAuthorization_WithUnauthenticatedUser_ShouldReturn401ForApi()
        {
            var identity = new ClaimsIdentity(); // not authenticated
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/api/test");
            var attr = new RoleAuthorizeAttribute(UserType.Admin);

            attr.OnAuthorization(context);

            context.Result.Should().NotBeNull();
            var jsonResult = context.Result.As<JsonResult>();
            jsonResult.StatusCode.Should().Be(401);
        }

        [Fact]
        public void OnAuthorization_WithUnauthenticatedUser_ShouldChallengeForMvc()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/home/index");
            var attr = new RoleAuthorizeAttribute(UserType.Admin);

            attr.OnAuthorization(context);

            context.Result.Should().BeOfType<ChallengeResult>();
        }

        [Fact]
        public void OnAuthorization_WithWrongRole_ShouldReturn403ForApi()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, UserType.User.ToString())
            }, "auth");
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/api/admin");
            var attr = new RoleAuthorizeAttribute(UserType.Admin);

            attr.OnAuthorization(context);

            context.Result.Should().NotBeNull();
            var jsonResult = context.Result.As<JsonResult>();
            jsonResult.StatusCode.Should().Be(403);
        }

        [Fact]
        public void OnAuthorization_WithCorrectRole_ShouldAllow()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, UserType.Admin.ToString())
            }, "auth");
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/api/admin");
            var attr = new RoleAuthorizeAttribute(UserType.Admin);

            attr.OnAuthorization(context);

            context.Result.Should().BeNull();
        }

        [Fact]
        public void OnAuthorization_WithMultipleRoles_ShouldAllowWhenAnyMatches()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, UserType.User.ToString())
            }, "auth");
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/api/test");
            var attr = new RoleAuthorizeAttribute(UserType.Admin, UserType.User);

            attr.OnAuthorization(context);

            context.Result.Should().BeNull();
        }

        [Fact]
        public void OnAuthorization_WithWrongRole_ShouldForbidForMvc()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, UserType.User.ToString())
            }, "auth");
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/home/admin");

            var attr = new RoleAuthorizeAttribute(UserType.Admin);
            attr.OnAuthorization(context);

            context.Result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void OnAuthorization_WithNoRolesSpecified_ShouldAllowAuthenticated()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            }, "auth");
            var user = new ClaimsPrincipal(identity);
            var context = CreateContext(user, "/api/test");
            var attr = new RoleAuthorizeAttribute(); // no roles

            attr.OnAuthorization(context);

            context.Result.Should().BeNull();
        }
    }
}
