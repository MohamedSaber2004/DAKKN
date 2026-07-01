using DAKKN.Application.Localization;
using Microsoft.Extensions.Localization;

namespace DAKKN.Tests.Tests.Localization
{
    public class JsonStringLocalizerFactoryTests
    {
        private readonly JsonStringLocalizerFactory _factory;

        public JsonStringLocalizerFactoryTests()
        {
            _factory = new JsonStringLocalizerFactory();
        }

        [Fact]
        public void Create_ByType_ShouldReturnJsonStringLocalizer()
        {
            var localizer = _factory.Create(typeof(Messages));
            localizer.Should().NotBeNull();
            localizer.Should().BeOfType<JsonStringLocalizer>();
        }

        [Fact]
        public void Create_ByType_ShouldReturnDifferentInstanceEachCall()
        {
            var localizer1 = _factory.Create(typeof(Messages));
            var localizer2 = _factory.Create(typeof(Messages));
            localizer1.Should().NotBeSameAs(localizer2);
        }

        [Fact]
        public void Create_ByBaseNameAndLocation_ShouldReturnJsonStringLocalizer()
        {
            var localizer = _factory.Create("Resources", "DAKKN.Application");
            localizer.Should().NotBeNull();
            localizer.Should().BeOfType<JsonStringLocalizer>();
        }

        [Fact]
        public void Create_ByBaseNameAndLocation_ShouldReturnDifferentInstanceEachCall()
        {
            var localizer1 = _factory.Create("Resources", "DAKKN.Application");
            var localizer2 = _factory.Create("Resources", "DAKKN.Application");
            localizer1.Should().NotBeSameAs(localizer2);
        }

        [Fact]
        public void Create_WithDifferentTypes_ShouldAllReturnJsonStringLocalizer()
        {
            var localizer1 = _factory.Create(typeof(Messages));
            var localizer2 = _factory.Create(typeof(string));
            localizer1.Should().BeOfType<JsonStringLocalizer>();
            localizer2.Should().BeOfType<JsonStringLocalizer>();
        }

        [Fact]
        public void Factory_ShouldImplementIStringLocalizerFactory()
        {
            _factory.Should().BeAssignableTo<IStringLocalizerFactory>();
        }
    }
}
