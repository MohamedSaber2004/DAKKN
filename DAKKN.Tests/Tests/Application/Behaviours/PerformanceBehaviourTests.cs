using DAKKN.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DAKKN.Tests.Tests.Application.Behaviours
{
    public class PerformanceBehaviourTests
    {
        [Fact]
        public async Task Handle_ShouldNotWarn_WhenUnderThreshold()
        {
            var loggerMock = new Mock<ILogger<TestRequest>>();
            var behaviour = new PerformanceBehaviour<TestRequest, string>(loggerMock.Object);

            var result = await behaviour.Handle(
                new TestRequest(),
                () => Task.FromResult("fast"),
                CancellationToken.None);

            result.Should().Be("fast");
        }

        [Fact]
        public async Task Handle_ShouldWarn_WhenOverThreshold()
        {
            var loggerMock = new Mock<ILogger<TestRequest>>();
            var behaviour = new PerformanceBehaviour<TestRequest, string>(loggerMock.Object);

            var result = await behaviour.Handle(
                new TestRequest(),
                async () =>
                {
                    await Task.Delay(700);
                    return "slow";
                },
                CancellationToken.None);

            result.Should().Be("slow");
        }

        public record TestRequest : IRequest<string>;
    }
}
