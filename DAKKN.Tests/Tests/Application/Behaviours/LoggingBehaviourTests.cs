using DAKKN.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAKKN.Tests.Tests.Application.Behaviours
{
    public class LoggingBehaviourTests
    {
        [Fact]
        public async Task Handle_ShouldLogAndPassThrough()
        {
            var loggerMock = new Mock<ILogger<LoggingBehaviour<TestRequest, string>>>();
            var behaviour = new LoggingBehaviour<TestRequest, string>(loggerMock.Object);

            var result = await behaviour.Handle(
                new TestRequest { Value = "test" },
                () => Task.FromResult("done"),
                CancellationToken.None);

            result.Should().Be("done");
        }

        [Fact]
        public async Task Handle_ShouldNotThrow_WhenSerializationFails()
        {
            var loggerMock = new Mock<ILogger<LoggingBehaviour<TestRequest, string>>>();
            var behaviour = new LoggingBehaviour<TestRequest, string>(loggerMock.Object);

            var result = await behaviour.Handle(
                new TestRequest { Value = null! },
                () => Task.FromResult("done"),
                CancellationToken.None);

            result.Should().Be("done");
        }

        public record TestRequest : IRequest<string>
        {
            public string Value { get; init; } = string.Empty;
        }
    }
}
