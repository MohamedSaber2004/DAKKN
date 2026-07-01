using DAKKN.Application.Common.Models;

namespace DAKKN.Tests.Tests.Application.Models
{
    public class ApiResponseTests
    {
        [Fact]
        public void Ok_ShouldReturnSuccessResponse()
        {
            var result = ApiResponse<string>.Ok("data");

            result.Success.Should().BeTrue();
            result.Data.Should().Be("data");
            result.StatusCode.Should().Be(200);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Ok_WithMessage_ShouldSetMessage()
        {
            var result = ApiResponse<int>.Ok(42, "All good");
            result.Message.Should().Be("All good");
            result.Data.Should().Be(42);
        }

        [Fact]
        public void Ok_WithCustomStatusCode_ShouldSetStatusCode()
        {
            var result = ApiResponse<object>.Ok(null, null, 201);
            result.StatusCode.Should().Be(201);
        }

        [Fact]
        public void Error_ShouldReturnErrorResponse()
        {
            var result = ApiResponse<string>.Error("Something went wrong", 400);

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Something went wrong");
            result.StatusCode.Should().Be(400);
            result.Data.Should().BeNull();
        }

        [Fact]
        public void Error_WithDictionary_ShouldStoreErrors()
        {
            var errors = new Dictionary<string, string[]>
            {
                { "field", new[] { "error" } }
            };

            var result = ApiResponse<object>.Error((IDictionary<string, string[]>)errors, "Validation failed");
            result.Errors.Should().ContainKey("field");
            result.Message.Should().Be("Validation failed");
        }

        [Fact]
        public void Error_Default_ShouldReturnStatusCode400()
        {
            var result = ApiResponse<object>.Error((IDictionary<string, string[]>)null);
            result.StatusCode.Should().Be(400);
            result.Success.Should().BeFalse();
        }
    }
}
