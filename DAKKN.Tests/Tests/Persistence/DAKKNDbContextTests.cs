using DAKKN.Domain.Entities;
using DAKKN.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Tests.Tests.Persistence
{
    public class DAKKNDbContextTests
    {
        private DAKKNDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DAKKNDbContext>()
                .UseInMemoryDatabase($"DB_Test_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.InvalidIncludePathError))
                .Options;
            return new DAKKNDbContext(options);
        }

        [Fact]
        public void DbSets_ShouldBeInitialized()
        {
            using var context = CreateContext();

            context.UserSettings.Should().NotBeNull();
            context.Products.Should().NotBeNull();
            context.Categories.Should().NotBeNull();
            context.ShippingGovernorates.Should().NotBeNull();
            context.InventoryTransactions.Should().NotBeNull();
            context.SystemSettings.Should().NotBeNull();
            context.LandingPageSettings.Should().NotBeNull();
            context.UserFavorites.Should().NotBeNull();
            context.Orders.Should().NotBeNull();
            context.OrderItems.Should().NotBeNull();
            context.OrderStatusHistories.Should().NotBeNull();
            context.BrandReviews.Should().NotBeNull();
            context.StickerSuggestions.Should().NotBeNull();
            context.SupportTickets.Should().NotBeNull();
            context.SupportReplies.Should().NotBeNull();
            context.SupportAttachments.Should().NotBeNull();
            context.SupportCategories.Should().NotBeNull();
            context.SupportFAQs.Should().NotBeNull();
            context.SupportFAQCategories.Should().NotBeNull();
            context.SupportActivities.Should().NotBeNull();
            context.SupportInternalNotes.Should().NotBeNull();
            context.SupportSettings.Should().NotBeNull();
        }

        [Fact]
        public async Task DbSets_ShouldAcceptEntities()
        {
            using var context = CreateContext();

            context.Products.Add(new Product
            {
                Name = "Test Product",
                ArName = "منتج اختبار",
                Description = "Desc",
                ArDescription = "وصف",
                Price = 100,
                ImageUrl = "/img.jpg",
                CategoryId = Guid.NewGuid()
            });

            var saved = await context.SaveChangesAsync();
            saved.Should().Be(1);
        }

        [Fact]
        public async Task Categories_ShouldStoreAndRetrieve()
        {
            using var context = CreateContext();
            var category = new Category
            {
                CategoryName = "Stickers",
                ArName = "ملصقات"
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var retrieved = await context.Categories.FindAsync(category.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CategoryName.Should().Be("Stickers");
        }

        [Fact]
        public async Task Products_ShouldStoreAndRetrieve()
        {
            using var context = CreateContext();
            var category = new Category { CategoryName = "Cat", ArName = "تصنيف" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product
            {
                Name = "Product A",
                ArName = "منتج أ",
                Description = "Description",
                ArDescription = "وصف",
                Price = 50.99m,
                ImageUrl = "/images/a.jpg",
                CategoryId = category.Id
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var retrieved = await context.Products.FindAsync(product.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Price.Should().Be(50.99m);
        }

        [Fact]
        public void OnModelCreating_ShouldApplyConfigurations()
        {
            using var context = CreateContext();
            var model = context.Model;

            var productEntity = model.FindEntityType(typeof(Product));
            productEntity.Should().NotBeNull();
            productEntity!.GetTableName().Should().Be("Products");

            var categoryEntity = model.FindEntityType(typeof(Category));
            categoryEntity.Should().NotBeNull();
            categoryEntity!.GetTableName().Should().Be("Categories");
        }

        [Fact]
        public void OnModelCreating_ShouldSetDefaultSchema()
        {
            using var context = CreateContext();
            var model = context.Model;
            model.GetDefaultSchema().Should().Be("dbo");
        }

        [Fact]
        public async Task SystemSettings_ShouldStoreAndRetrieve()
        {
            using var context = CreateContext();
            var setting = new SystemSetting
            {
                Key = "site_name",
                Value = "DAKKN Store"
            };
            context.SystemSettings.Add(setting);
            await context.SaveChangesAsync();

            var retrieved = await context.SystemSettings.FindAsync(setting.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Key.Should().Be("site_name");
        }

        [Fact]
        public async Task Orders_ShouldStoreAndRetrieve()
        {
            using var context = CreateContext();
            var govId = Guid.NewGuid();
            var order = new Order(
                "Test Customer", "test@test.com", "0123456789",
                "Test Address", govId, "Damietta",
                30m, 100m, null, null);
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var retrieved = await context.Orders.FindAsync(order.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CustomerName.Should().Be("Test Customer");
        }

        [Fact]
        public async Task BrandReviews_ShouldStoreAndRetrieve()
        {
            using var context = CreateContext();
            var review = new BrandReview
            {
                ReviewText = "Great!",
                Rating = 5
            };
            context.BrandReviews.Add(review);
            await context.SaveChangesAsync();

            var retrieved = await context.BrandReviews.FindAsync(review.Id);
            retrieved.Should().NotBeNull();
            retrieved!.ReviewText.Should().Be("Great!");
        }
    }
}
