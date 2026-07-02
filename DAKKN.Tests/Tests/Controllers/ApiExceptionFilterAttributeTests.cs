using DAKKN.Appearence.Filters;
using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace DAKKN.Tests.Tests.Controllers
{
    public class ApiExceptionFilterAttributeTests
    {
        private readonly Mock<ILogger<ApiExceptionFilterAttribute>> _loggerMock;
        private readonly Mock<IStringLocalizer<Messages>> _localizerMock;
        private readonly ApiExceptionFilterAttribute _filter;

        public ApiExceptionFilterAttributeTests()
        {
            _loggerMock = new Mock<ILogger<ApiExceptionFilterAttribute>>();
            _localizerMock = new Mock<IStringLocalizer<Messages>>();
            _localizerMock.Setup(x => x[It.IsAny<string>()]).Returns((string key) => new LocalizedString(key, $"[{key}]"));
            _filter = new ApiExceptionFilterAttribute(_loggerMock.Object, _localizerMock.Object);
        }

        private ExceptionContext CreateContext(Exception exception)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/test";
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }

        [Fact]
        public void OnException_WithNotFoundException_ShouldReturn404()
        {
            var context = CreateContext(new NotFoundException("Entity not found"));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<NotFoundObjectResult>();
            result.StatusCode.Should().Be(404);
            var response = result.Value.As<ApiResponse<object>>();
            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(404);
        }

        [Fact]
        public void OnException_WithValidationException_ShouldReturn400()
        {
            var errors = new Dictionary<string, string[]> { { "Name", new[] { "Name is required" } } };
            var context = CreateContext(new ValidationException(errors));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
            var response = result.Value.As<ApiResponse<object>>();
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainKey("Name");
        }

        [Fact]
        public void OnException_WithBadRequestException_ShouldReturn400()
        {
            var context = CreateContext(new BadRequestException("Invalid input"));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<BadRequestObjectResult>();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void OnException_WithUnAuthorizedException_ShouldReturn401()
        {
            var context = CreateContext(new UnAuthorizedException("Not authorized"));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<UnauthorizedObjectResult>();
            result.StatusCode.Should().Be(401);
            var response = result.Value.As<ApiResponse<object>>();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public void OnException_WithUnknownException_ShouldReturn500()
        {
            var context = CreateContext(new InvalidOperationException("Something went wrong"));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<ObjectResult>();
            result.StatusCode.Should().Be(500);
            var response = result.Value.As<ApiResponse<object>>();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public void OnException_WithDbUpdateConcurrencyException_ShouldReturn409()
        {
            var context = CreateContext(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Concurrency conflict"));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var result = context.Result.As<ObjectResult>();
            result.StatusCode.Should().Be(409);
        }

        [Fact]
        public void OnException_WithNotFoundAndNoMessage_ShouldUseLocalizedMessage()
        {
            var context = CreateContext(new NotFoundException(""));
            _filter.OnException(context);

            context.ExceptionHandled.Should().BeTrue();
            var response = context.Result.As<NotFoundObjectResult>().Value.As<ApiResponse<object>>();
            response.Message.Should().Be("[exception.not_found]");
        }

        [Fact]
        public void OnException_ShouldLogUnknownException()
        {
            var context = CreateContext(new Exception("Unexpected"));
            _filter.OnException(context);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public void OnException_WithMvcViewPath_ShouldNotHandleException()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/admin/settings";
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new NotFoundException("Not found")
            };

            _filter.OnException(context);

            context.ExceptionHandled.Should().BeFalse();
            context.Result.Should().BeNull();
        }
    }
}
