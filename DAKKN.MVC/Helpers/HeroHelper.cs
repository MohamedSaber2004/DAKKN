using System.Text.Json;
using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.MVC.Helpers
{
    public static class HeroHelper
    {
        private static readonly JsonSerializerOptions CaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static HeroSettingsViewModel Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "{}")
                return new HeroSettingsViewModel();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (IsLegacyFormat(root))
                    return MigrateFromLegacy(root);

                return JsonSerializer.Deserialize<HeroSettingsViewModel>(json, CaseInsensitive) ?? new();
            }
            catch
            {
                return new HeroSettingsViewModel();
            }
        }

        private static bool IsLegacyFormat(JsonElement root)
        {
            return root.TryGetProperty("Badge", out _) || root.TryGetProperty("badge", out _);
        }

        private static HeroSettingsViewModel MigrateFromLegacy(JsonElement root)
        {
            var value = GetString(root, "Badge") ?? GetString(root, "badge") ?? string.Empty;
            var titleA = GetString(root, "TitleA") ?? GetString(root, "titleA") ?? string.Empty;
            var titleB = GetString(root, "TitleB") ?? GetString(root, "titleB") ?? string.Empty;
            var titleC = GetString(root, "TitleC") ?? GetString(root, "titleC") ?? string.Empty;
            var titleD = GetString(root, "TitleD") ?? GetString(root, "titleD") ?? string.Empty;
            var description = GetString(root, "Description") ?? GetString(root, "description") ?? string.Empty;
            var buttonText = GetString(root, "ButtonText") ?? GetString(root, "buttonText") ?? string.Empty;
            var imageUrl = GetString(root, "ImageUrl") ?? GetString(root, "imageUrl") ?? string.Empty;

            return new HeroSettingsViewModel
            {
                BadgeAr = value,
                BadgeEn = value,
                TitleAAr = titleA,
                TitleAEn = titleA,
                TitleBAr = titleB,
                TitleBEn = titleB,
                TitleCAr = titleC,
                TitleCEn = titleC,
                TitleDAr = titleD,
                TitleDEn = titleD,
                DescriptionAr = description,
                DescriptionEn = description,
                ButtonTextAr = buttonText,
                ButtonTextEn = buttonText,
                ImageUrl = imageUrl
            };
        }

        private static string? GetString(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String)
                return prop.GetString();
            return null;
        }
    }
}
