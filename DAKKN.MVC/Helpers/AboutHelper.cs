using System.Text.Json;
using DAKKN.MVC.ViewModels.Admin;

namespace DAKKN.MVC.Helpers
{
    public static class AboutHelper
    {
        private static readonly JsonSerializerOptions CaseInsensitive = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static AboutSettingsViewModel Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json) || json == "{}")
                return new AboutSettingsViewModel();

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (IsLegacyFormat(root))
                    return MigrateFromLegacy(root);

                return JsonSerializer.Deserialize<AboutSettingsViewModel>(json, CaseInsensitive) ?? new();
            }
            catch
            {
                return new AboutSettingsViewModel();
            }
        }

        private static bool IsLegacyFormat(JsonElement root)
        {
            return root.TryGetProperty("Title", out _) || root.TryGetProperty("title", out _);
        }

        private static AboutSettingsViewModel MigrateFromLegacy(JsonElement root)
        {
            var title = GetString(root, "Title") ?? GetString(root, "title") ?? string.Empty;
            var desc = GetString(root, "Description") ?? GetString(root, "description") ?? string.Empty;
            var f1t = GetString(root, "Feature1Title") ?? GetString(root, "feature1Title") ?? string.Empty;
            var f1d = GetString(root, "Feature1Desc") ?? GetString(root, "feature1Desc") ?? string.Empty;
            var f2t = GetString(root, "Feature2Title") ?? GetString(root, "feature2Title") ?? string.Empty;
            var f2d = GetString(root, "Feature2Desc") ?? GetString(root, "feature2Desc") ?? string.Empty;
            var f3t = GetString(root, "Feature3Title") ?? GetString(root, "feature3Title") ?? string.Empty;
            var f3d = GetString(root, "Feature3Desc") ?? GetString(root, "feature3Desc") ?? string.Empty;

            return new AboutSettingsViewModel
            {
                TitleAr = title, TitleEn = title,
                DescriptionAr = desc, DescriptionEn = desc,
                Feature1TitleAr = f1t, Feature1TitleEn = f1t,
                Feature1DescAr = f1d, Feature1DescEn = f1d,
                Feature2TitleAr = f2t, Feature2TitleEn = f2t,
                Feature2DescAr = f2d, Feature2DescEn = f2d,
                Feature3TitleAr = f3t, Feature3TitleEn = f3t,
                Feature3DescAr = f3d, Feature3DescEn = f3d,
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
