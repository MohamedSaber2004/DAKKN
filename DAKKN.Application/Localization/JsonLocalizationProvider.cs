using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace DAKKN.Application.Localization
{
    public class JsonLocalizationProvider
    {
        private static readonly Dictionary<string, Dictionary<string, string>> _localizations = new(StringComparer.OrdinalIgnoreCase);

        public static void Initialize(string? rootPath = null)
        {
            var baseDirectory = rootPath ?? AppContext.BaseDirectory;
            var assemblyLocation = Path.GetDirectoryName(typeof(JsonLocalizationProvider).Assembly.Location);

            // Try different possible resource paths
            var possiblePaths = new List<string>();

            // 1. Check bin folder (most reliable for deployed apps)
            possiblePaths.Add(Path.Combine(baseDirectory, "Localization", "Resources"));
            
            // 2. Check source folders (for development)
            if (!string.IsNullOrEmpty(rootPath))
            {
                possiblePaths.Add(Path.Combine(rootPath, "Localization", "Resources"));
                possiblePaths.Add(Path.Combine(rootPath, "..", "DAKKN.Application", "Localization", "Resources"));
                
                // If we are in DAKKN.MVC, go up to solution root then into DAKKN.Application
                var solutionRoot = Directory.GetParent(rootPath)?.FullName;
                if (solutionRoot != null)
                {
                    possiblePaths.Add(Path.Combine(solutionRoot, "DAKKN.Application", "Localization", "Resources"));
                }
            }

            possiblePaths.Add(Path.Combine(assemblyLocation ?? "", "Localization", "Resources"));
            possiblePaths.Add(Path.Combine(Directory.GetCurrentDirectory(), "Localization", "Resources"));

            var resourcePath = possiblePaths.FirstOrDefault(Directory.Exists);

            if (resourcePath == null)
            {
                Debug.WriteLine("Warning: Localization resource directory not found in any expected location.");
                Debug.WriteLine("Paths tried:");
                foreach (var path in possiblePaths) Debug.WriteLine($" - {path}");
                return;
            }

            Debug.WriteLine($"Loading localization resources from: {resourcePath}");

            var cultures = new[] { "en", "ar" };

            foreach (var culture in cultures)
            {
                var filePath = Path.Combine(resourcePath, $"messages.{culture}.json");
                if (File.Exists(filePath))
                {
                    try
                    {
                        var json = File.ReadAllText(filePath);
                        using var doc = JsonDocument.Parse(json);
                        var cultureData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                        FlattenJson(doc.RootElement, "", cultureData);

                        _localizations[culture] = cultureData;
                        Debug.WriteLine($"Successfully loaded {cultureData.Count} keys for culture: {culture}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error loading localization file {filePath}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Warning: Localization file not found: {filePath}");
                }
            }
        }

        private static void FlattenJson(JsonElement element, string prefix, Dictionary<string, string> result)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var name = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                        FlattenJson(property.Value, name, result);
                    }
                    break;
                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        FlattenJson(item, $"{prefix}[{index}]", result);
                        index++;
                    }
                    break;
                case JsonValueKind.String:
                    result[prefix] = element.GetString() ?? "";
                    break;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    result[prefix] = element.ToString();
                    break;
            }
        }

        public static string GetLocalizedString(string key, string? culture = null)
        {
            // Use CurrentUICulture as it's standard for string resources in ASP.NET Core
            culture ??= CultureInfo.CurrentUICulture.Name;

            // Normalize culture to 2 letters (e.g., "ar-EG" -> "ar")
            if (!string.IsNullOrEmpty(culture) && culture.Contains('-'))
            {
                culture = culture.Split('-')[0];
            }
            else if (!string.IsNullOrEmpty(culture) && culture.Length > 2)
            {
                culture = culture.Substring(0, 2);
            }

            // Explicitly default to 'ar' if culture is null or not supported (other than 'en')
            if (string.IsNullOrEmpty(culture) || (culture != "en" && culture != "ar"))
            {
                culture = "ar";
            }

            if (_localizations.TryGetValue(culture, out var cultureData) && cultureData.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback to English if translation not found
            if (culture != "en" && _localizations.TryGetValue("en", out var enData) && enData.TryGetValue(key, out var enValue))
            {
                return enValue;
            }

            return key; // Return key if translation not found
        }

        public static string GetLocalizedString(string key, string? culture, params object[] args)
        {
            var baseValue = GetLocalizedString(key, culture);

            try
            {
                return string.Format(baseValue, args);
            }
            catch
            {
                return baseValue;
            }
        }

        public static IDictionary<string, string> GetTranslations(string? culture = null)
        {
            culture ??= CultureInfo.CurrentUICulture.Name;
            
            if (!string.IsNullOrEmpty(culture) && culture.Contains('-'))
                culture = culture.Split('-')[0];

            if (string.IsNullOrEmpty(culture) || (culture != "en" && culture != "ar"))
                culture = "ar";

            if (_localizations.TryGetValue(culture, out var cultureData))
            {
                return cultureData;
            }

            return new Dictionary<string, string>();
        }
    }
}
