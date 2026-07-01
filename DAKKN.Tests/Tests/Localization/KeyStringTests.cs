using DAKKN.Application.Localization;

namespace DAKKN.Tests.Tests.Localization
{
    public class KeyStringTests : IDisposable
    {
        private readonly string _tempDir;

        public KeyStringTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"DAKKN_KeyString_{Guid.NewGuid()}");
            var resourcesDir = Path.Combine(_tempDir, "Localization", "Resources");
            Directory.CreateDirectory(resourcesDir);

            var enJson = """{ "greeting": "Hello", "farewell": "Goodbye" }""";
            File.WriteAllText(Path.Combine(resourcesDir, "messages.en.json"), enJson);

            var arJson = """{ "greeting": "مرحبا" }""";
            File.WriteAllText(Path.Combine(resourcesDir, "messages.ar.json"), arJson);

            JsonLocalizationProvider.Initialize(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        [Fact]
        public void Constructor_ShouldStoreKey()
        {
            var ks = new KeyString("test.key");
            ks.Key.Should().Be("test.key");
        }

        [Fact]
        public void Value_ShouldReturnLocalizedString()
        {
            var ks = new KeyString("greeting");
            ks.Value.Should().Be("Hello");
        }

        [Fact]
        public void Value_WhenKeyNotFound_ShouldReturnKey()
        {
            var ks = new KeyString("nonexistent.key");
            ks.Value.Should().Be("nonexistent.key");
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldReturnLocalizedValue()
        {
            var ks = new KeyString("greeting");
            string result = ks;
            result.Should().Be("Hello");
        }

        [Fact]
        public void ImplicitConversion_ToString_WhenKeyNotFound_ShouldReturnKey()
        {
            var ks = new KeyString("missing");
            string result = ks;
            result.Should().Be("missing");
        }

        [Fact]
        public void ToString_ShouldReturnLocalizedValue()
        {
            var ks = new KeyString("greeting");
            ks.ToString().Should().Be("Hello");
        }

        [Fact]
        public void ToString_WhenKeyNotFound_ShouldReturnKey()
        {
            var ks = new KeyString("unknown");
            ks.ToString().Should().Be("unknown");
        }

        [Fact]
        public void GetLocalizedValue_ShouldReturnValue()
        {
            var ks = new KeyString("farewell");
            ks.GetLocalizedValue().Should().Be("Goodbye");
        }

        [Fact]
        public void Key_ShouldReturnOriginalKey()
        {
            var ks = new KeyString("farewell");
            ks.Key.Should().Be("farewell");
        }

        [Fact]
        public void ImplicitConversion_WithNull_ShouldReturnEmpty()
        {
            KeyString? ks = null;
            string result = ks!;
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void MultipleKeys_ShouldAllResolve()
        {
            var greeting = new KeyString("greeting");
            var farewell = new KeyString("farewell");
            greeting.Value.Should().Be("Hello");
            farewell.Value.Should().Be("Goodbye");
        }
    }
}
