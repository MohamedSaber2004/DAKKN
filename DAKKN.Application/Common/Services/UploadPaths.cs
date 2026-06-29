using DAKKN.Application.Common.Options;
using Microsoft.Extensions.Configuration;

namespace DAKKN.Application.Common.Services
{
    public class UploadPaths
    {
        private static UploadPathsOptions? Options;

        public static void Configure(IConfiguration configuration)
        {
            Options = configuration.GetSection("UploadPaths").Get<UploadPathsOptions>();
        }

        public static string? Root => Options?.Root;
        public static string? Products => Options?.Products;
        public static string? Categories => Options?.Categories;
        public static string? Users => Options?.Users;
        public static string? Invoices => Options?.Invoices;
        public static string? Reviews => Options?.Reviews;
        public static string? Suggestions => Options?.Suggestions;

        public static string? GetPath(int place)
        {
            return place switch
            {
                0 => Root,
                1 => Products,
                2 => Categories,
                3 => Users,
                4 => Invoices,
                5 => Reviews,
                6 => Suggestions,
                _ => string.Empty
            };
        }

        public static IEnumerable<string> GetAllPaths()
        {
            if (Options is null) yield break;
            yield return Options.Root;
            yield return Options.Products;
            yield return Options.Categories;
            yield return Options.Users;
            yield return Options.Invoices;
            yield return Options.Reviews;
            yield return Options.Suggestions;
        }
    }
}
