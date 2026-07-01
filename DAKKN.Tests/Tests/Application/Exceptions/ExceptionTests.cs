using DAKKN.Application.Common.Exceptions;
using FluentValidation.Results;

namespace DAKKN.Tests.Tests.Application.Exceptions
{
    public class ExceptionTests
    {
        [Fact]
        public void ValidationException_ShouldStoreErrorsFromFailures()
        {
            var failures = new List<ValidationFailure>
            {
                new("Name", "Name is required"),
                new("Name", "Name is too long"),
                new("Email", "Email is invalid")
            };

            var ex = new ValidationException(failures);

            ex.Errors.Should().ContainKey("Name");
            ex.Errors["Name"].Should().HaveCount(2);
            ex.Errors.Should().ContainKey("Email");
            ex.Message.Should().Be("One or more validation failures");
        }

        [Fact]
        public void ValidationException_ShouldStoreErrorsFromDictionary()
        {
            var errors = new Dictionary<string, string[]>
            {
                { "Field1", new[] { "Error 1" } }
            };

            var ex = new ValidationException(errors);
            ex.Errors.Should().ContainKey("Field1");
        }

        [Fact]
        public void ValidationException_Default_ShouldHaveEmptyErrors()
        {
            var ex = new ValidationException();
            ex.Errors.Should().BeEmpty();
        }

        [Fact]
        public void NotFoundException_ShouldStoreMessage()
        {
            var ex = new NotFoundException("Entity not found");
            ex.Message.Should().Be("Entity not found");
        }

        [Fact]
        public void NotFoundException_WithNameAndKey_ShouldFormatMessage()
        {
            var ex = new NotFoundException("Product", Guid.Empty);
            ex.Message.Should().Be("Entity \"Product\" (00000000-0000-0000-0000-000000000000) was not found.");
        }

        [Fact]
        public void BadRequestException_WithMessage_ShouldStoreError()
        {
            var ex = new BadRequestException("Invalid request");
            ex.Message.Should().Be("Invalid request");
            ex.Errors.Should().ContainKey("");
            ex.Errors[""].Should().Contain("Invalid request");
        }

        [Fact]
        public void BadRequestException_WithStringArray_ShouldStoreErrors()
        {
            var ex = new BadRequestException(new[] { "Error1", "Error2" });
            ex.Errors[""].Should().BeEquivalentTo(new[] { "Error1", "Error2" });
        }

        [Fact]
        public void BadRequestException_Default_ShouldHaveEmptyErrors()
        {
            var ex = new BadRequestException();
            ex.Errors.Should().BeEmpty();
        }

        [Fact]
        public void UnAuthorizedException_ShouldStoreMessage()
        {
            var ex = new UnAuthorizedException("Access denied");
            ex.Message.Should().Be("Access denied");
        }
    }
}
