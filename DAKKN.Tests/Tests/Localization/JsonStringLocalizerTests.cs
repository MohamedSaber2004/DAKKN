using DAKKN.Application.Localization;

namespace DAKKN.Tests.Tests.Localization
{
    public class JsonStringLocalizerTests : IDisposable
    {
        private readonly string _tempDir;

        public JsonStringLocalizerTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"DAKKN_Localizer_{Guid.NewGuid()}");
            var resourcesDir = Path.Combine(_tempDir, "Localization", "Resources");
            Directory.CreateDirectory(resourcesDir);

            var enJson = """{ "greeting": "Hello", "farewell": "Goodbye" }""";
            File.WriteAllText(Path.Combine(resourcesDir, "messages.en.json"), enJson);
            JsonLocalizationProvider.Initialize(_tempDir);
        }

        [Fact]
        public void Indexer_ShouldReturnLocalizedString_WhenKeyExists()
        {
            var localizer = new JsonStringLocalizer("en");
            var result = localizer["greeting"];
            result.Value.Should().Be("Hello");
            result.ResourceNotFound.Should().BeFalse();
        }

        [Fact]
        public void Indexer_ShouldReturnKey_WhenKeyNotFound()
        {
            var localizer = new JsonStringLocalizer("en");
            var result = localizer["missing_key"];
            result.Value.Should().Be("missing_key");
            result.ResourceNotFound.Should().BeFalse();
        }

        [Fact]
        public void Indexer_WithArgs_ShouldFormatString()
        {
            var resourcesDir = Path.Combine(_tempDir, "Localization", "Resources");
            var json = """{ "welcome": "Welcome, {0}!" }""";
            File.WriteAllText(Path.Combine(resourcesDir, "messages.en.json"), json);
            JsonLocalizationProvider.Initialize(_tempDir);

            var localizer = new JsonStringLocalizer("en");
            var result = localizer["welcome", "Alice"];
            result.Value.Should().Be("Welcome, Alice!");
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
    }
}
