using DAKKN.Application.Localization;

namespace DAKKN.Tests.Tests.Localization
{
    public class JsonLocalizationProviderTests : IDisposable
    {
        private readonly string _tempDir;

        public JsonLocalizationProviderTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"DAKKN_Localization_{Guid.NewGuid()}");
            var resourcesDir = Path.Combine(_tempDir, "Localization", "Resources");
            Directory.CreateDirectory(resourcesDir);

            var arJson = """
            {
                "auth": {
                    "password_mismatch": "كلمة المرور غير متطابقة",
                    "invalid_credentials": "بيانات الدخول غير صحيحة"
                },
                "common": {
                    "save": "حفظ",
                    "cancel": "إلغاء"
                }
            }
            """;

            var enJson = """
            {
                "auth": {
                    "password_mismatch": "Password mismatch",
                    "invalid_credentials": "Invalid credentials"
                },
                "common": {
                    "save": "Save",
                    "cancel": "Cancel"
                }
            }
            """;

            File.WriteAllText(Path.Combine(resourcesDir, "messages.ar.json"), arJson);
            File.WriteAllText(Path.Combine(resourcesDir, "messages.en.json"), enJson);

            JsonLocalizationProvider.Initialize(_tempDir);
        }

        [Fact]
        public void GetLocalizedString_ShouldReturnArabic_WhenCultureIsAr()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("auth.password_mismatch", "ar");
            result.Should().Be("كلمة المرور غير متطابقة");
        }

        [Fact]
        public void GetLocalizedString_ShouldReturnEnglish_WhenCultureIsEn()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("auth.password_mismatch", "en");
            result.Should().Be("Password mismatch");
        }

        [Fact]
        public void GetLocalizedString_ShouldDefaultToArabic_WhenCultureIsUnsupported()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("common.save", "fr");
            result.Should().Be("حفظ");
        }

        [Fact]
        public void GetLocalizedString_ShouldDefaultToArabic_WhenCultureIsNull()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("common.save", null);
            var expected = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar")
                ? "حفظ" : "Save";
            result.Should().Be(expected);
        }

        [Fact]
        public void GetLocalizedString_ShouldFallbackToEnglish_WhenArabicKeyMissing()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("common.cancel", "en");
            result.Should().Be("Cancel");
        }

        [Fact]
        public void GetLocalizedString_ShouldReturnKey_WhenKeyNotFound()
        {
            var result = JsonLocalizationProvider.GetLocalizedString("nonexistent.key", "en");
            result.Should().Be("nonexistent.key");
        }

        [Fact]
        public void GetLocalizedString_WithFormatArgs_ShouldFormatCorrectly()
        {
            var enJson = """
            {
                "welcome": "Welcome, {0}!"
            }
            """;

            var resourcesDir = Path.Combine(_tempDir, "Localization", "Resources");
            File.WriteAllText(Path.Combine(resourcesDir, "messages.en.json"), enJson);
            JsonLocalizationProvider.Initialize(_tempDir);

            var result = JsonLocalizationProvider.GetLocalizedString("welcome", "en", "John");
            result.Should().Be("Welcome, John!");
        }

        [Fact]
        public void GetTranslations_ShouldReturnAllKeysForCulture()
        {
            var translations = JsonLocalizationProvider.GetTranslations("en");
            translations.Should().NotBeEmpty();
            translations.Values.Should().AllSatisfy(v => v.Should().NotBeNull());
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
    }
}
