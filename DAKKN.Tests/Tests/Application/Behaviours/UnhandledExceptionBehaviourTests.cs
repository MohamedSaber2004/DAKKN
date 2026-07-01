using DAKKN.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAKKN.Tests.Tests.Application.Behaviours
{
    public class UnhandledExceptionBehaviourTests
    {
        [Fact]
        public async Task Handle_ShouldPassThrough_WhenNoException()
        {
            var loggerMock = new Mock<ILogger<TestRequest>>();
            var behaviour = new UnhandledExceptionBehaviour<TestRequest, string>(loggerMock.Object);

            var result = await behaviour.Handle(
                new TestRequest(),
                () => Task.FromResult("success"),
                CancellationToken.None);

            result.Should().Be("success");
        }

        [Fact]
        public async Task Handle_ShouldRethrow_WhenExceptionOccurs()
        {
            var loggerMock = new Mock<ILogger<TestRequest>>();
            var behaviour = new UnhandledExceptionBehaviour<TestRequest, string>(loggerMock.Object);

            Func<Task> act = () => behaviour.Handle(
                new TestRequest(),
                () => throw new InvalidOperationException("test error"),
                CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("test error");
        }

        public record TestRequest : IRequest<string>;
    }
}
