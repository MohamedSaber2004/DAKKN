using DAKKN.Application.Common.Behaviours;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DAKKN.Tests.Tests.Application.Behaviours
{
    public class ValidationBehaviourTests
    {
        [Fact]
        public async Task Handle_ShouldCallNext_WhenNoValidators()
        {
            var behaviour = new ValidationBehaviour<TestRequest, string>(Enumerable.Empty<IValidator<TestRequest>>());
            var result = await behaviour.Handle(new TestRequest(), () => Task.FromResult("done"), CancellationToken.None);
            result.Should().Be("done");
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenValidationPasses()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var behaviour = new ValidationBehaviour<TestRequest, string>(new[] { validatorMock.Object });
            var result = await behaviour.Handle(new TestRequest(), () => Task.FromResult("ok"), CancellationToken.None);
            result.Should().Be("ok");
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenValidationFails()
        {
            var validatorMock = new Mock<IValidator<TestRequest>>();
            var failures = new List<ValidationFailure> { new("Name", "Required") };
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            var behaviour = new ValidationBehaviour<TestRequest, string>(new[] { validatorMock.Object });
            Func<Task> act = () => behaviour.Handle(new TestRequest(), () => Task.FromResult("ok"), CancellationToken.None);
            await act.Should().ThrowAsync<DAKKN.Application.Common.Exceptions.ValidationException>()
                .Where(ex => ex.Errors.ContainsKey("Name"));
        }

        public record TestRequest : IRequest<string>;
    }
}
