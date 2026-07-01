using DAKKN.Application.Common.Options;
using DAKKN.Application.Common.Services;
using Microsoft.Extensions.Configuration;

namespace DAKKN.Tests.Tests.Application.Services
{
    public class UploadPathsTests
    {
        [Fact]
        public void GetPath_ShouldReturnCorrectPath_ForEachPlace()
        {
            var configData = new Dictionary<string, string?>
            {
                { "UploadPaths:Root", "uploads" },
                { "UploadPaths:Products", "uploads/products" },
                { "UploadPaths:Categories", "uploads/categories" },
                { "UploadPaths:Users", "uploads/users" },
                { "UploadPaths:Invoices", "uploads/invoices" },
                { "UploadPaths:Reviews", "uploads/reviews" },
                { "UploadPaths:Suggestions", "uploads/suggestions" },
                { "UploadPaths:Support", "uploads/support" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            UploadPaths.Configure(config);

            UploadPaths.Root.Should().Be("uploads");
            UploadPaths.Products.Should().Be("uploads/products");
            UploadPaths.GetPath(0).Should().Be("uploads");
            UploadPaths.GetPath(1).Should().Be("uploads/products");
            UploadPaths.GetPath(2).Should().Be("uploads/categories");
            UploadPaths.GetPath(3).Should().Be("uploads/users");
            UploadPaths.GetPath(4).Should().Be("uploads/invoices");
            UploadPaths.GetPath(5).Should().Be("uploads/reviews");
            UploadPaths.GetPath(6).Should().Be("uploads/suggestions");
            UploadPaths.GetPath(7).Should().Be("uploads/support");
        }

        [Fact]
        public void GetPath_ShouldReturnEmpty_ForUnknownPlace()
        {
            UploadPaths.Configure(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()).Build());
            UploadPaths.GetPath(99).Should().Be(string.Empty);
        }

        [Fact]
        public void GetAllPaths_ShouldReturnAllConfiguredPaths()
        {
            var configData = new Dictionary<string, string?>
            {
                { "UploadPaths:Root", "root" },
                { "UploadPaths:Products", "products" },
                { "UploadPaths:Categories", "categories" },
                { "UploadPaths:Users", "users" },
                { "UploadPaths:Invoices", "invoices" },
                { "UploadPaths:Reviews", "reviews" },
                { "UploadPaths:Suggestions", "suggestions" },
                { "UploadPaths:Support", "support" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();

            UploadPaths.Configure(config);

            var paths = UploadPaths.GetAllPaths().ToList();
            paths.Should().HaveCount(8);
            paths.Should().Contain("root");
            paths.Should().Contain("support");
        }
    }
}
