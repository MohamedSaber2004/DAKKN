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
        public static string? Support => Options?.Support;
        public static string? CustomOrders => Options?.CustomOrders;

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
                7 => Support,
                8 => CustomOrders,
                _ => string.Empty
            };
        }

        public static IEnumerable<string> GetAllPaths()
        {
            if (Options is null) yield break;
            if (!string.IsNullOrWhiteSpace(Options.Root)) yield return Options.Root;
            if (!string.IsNullOrWhiteSpace(Options.Products)) yield return Options.Products;
            if (!string.IsNullOrWhiteSpace(Options.Categories)) yield return Options.Categories;
            if (!string.IsNullOrWhiteSpace(Options.Users)) yield return Options.Users;
            if (!string.IsNullOrWhiteSpace(Options.Invoices)) yield return Options.Invoices;
            if (!string.IsNullOrWhiteSpace(Options.Reviews)) yield return Options.Reviews;
            if (!string.IsNullOrWhiteSpace(Options.Suggestions)) yield return Options.Suggestions;
            if (!string.IsNullOrWhiteSpace(Options.Support)) yield return Options.Support;
            if (!string.IsNullOrWhiteSpace(Options.CustomOrders)) yield return Options.CustomOrders;
        }
    }
}
