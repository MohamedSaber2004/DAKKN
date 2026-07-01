using DAKKN.Appearence.Services;
using DAKKN.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace DAKKN.Tests.Tests.Controllers
{
    public class CurrentUserServiceTests
    {
        private CurrentUserService CreateService(ClaimsPrincipal? user, string? remoteIp = null)
        {
            var httpContext = new DefaultHttpContext();
            if (user != null)
                httpContext.User = user;
            if (remoteIp != null)
                httpContext.Connection.RemoteIpAddress = IPAddress.Parse(remoteIp);

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            return new CurrentUserService(accessorMock.Object);
        }

        [Fact]
        public void UserId_WhenUserAuthenticatedWithNameIdentifier_ShouldReturnGuid()
        {
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));

            service.UserId.Should().Be(userId);
        }

        [Fact]
        public void UserId_WhenUserAuthenticatedWithSubClaim_ShouldReturnGuid()
        {
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("sub", userId.ToString())
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));

            service.UserId.Should().Be(userId);
        }

        [Fact]
        public void UserId_WithNoUser_ShouldReturnEmpty()
        {
            var service = CreateService(null);
            service.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserId_WithUnauthenticatedUser_ShouldReturnEmpty()
        {
            var identity = new ClaimsIdentity();
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void UserId_WithNoMatchingClaim_ShouldReturnEmpty()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("other", "somevalue")
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void IsAuthenticated_WhenUserIsAuthenticated_ShouldReturnTrue()
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void IsAuthenticated_WhenNoUser_ShouldReturnFalse()
        {
            var service = CreateService(null);
            service.IsAuthenticated.Should().BeFalse();
        }

        [Fact]
        public void UserName_WhenFullNameClaimPresent_ShouldReturnIt()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("FullName", "John Doe")
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserName.Should().Be("John Doe");
        }

        [Fact]
        public void UserName_WhenNotAuthenticated_ShouldReturnSystem()
        {
            var identity = new ClaimsIdentity();
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserName.Should().Be("System");
        }

        [Fact]
        public void UserName_WhenNoHttpContext_ShouldReturnSystem()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);
            var service = new CurrentUserService(accessorMock.Object);
            service.UserName.Should().Be("System");
        }

        [Fact]
        public void IpAddress_WhenSet_ShouldReturnIp()
        {
            var service = CreateService(null, "192.168.1.1");
            service.IpAddress.Should().Be("192.168.1.1");
        }

        [Fact]
        public void IpAddress_WhenNotSet_ShouldBeNull()
        {
            var service = CreateService(null);
            service.IpAddress.Should().BeNull();
        }

        [Fact]
        public void UserType_WhenRoleClaimIsAdmin_ShouldReturnAdmin()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserType.Should().Be(UserType.Admin);
        }

        [Fact]
        public void UserType_WhenRoleClaimIsUser_ShouldReturnUser()
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "User")
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserType.Should().Be(UserType.User);
        }

        [Fact]
        public void UserType_WhenNotAuthenticated_ShouldReturnUser()
        {
            var identity = new ClaimsIdentity();
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserType.Should().Be(UserType.User);
        }

        [Fact]
        public void UserId_ShouldFallbackToFirstGuidClaim()
        {
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim("custom", "not-a-guid"),
                new Claim("uid", userId.ToString())
            }, "auth");
            var service = CreateService(new ClaimsPrincipal(identity));
            service.UserId.Should().Be(userId);
        }
    }
}
