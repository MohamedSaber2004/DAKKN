using DAKKN.Domain.Common;
using DAKKN.Domain.Entities;
using DAKKN.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAKKN.Tests.Tests.Persistence
{
    public class EntityTypeConfigurationTests
    {
        private static DAKKNDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DAKKNDbContext>()
                .UseInMemoryDatabase($"Config_Test_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.InvalidIncludePathError))
                .Options;
            return new DAKKNDbContext(options);
        }

        private static IEntityType GetEntityType<T>() where T : class
        {
            using var context = CreateContext();
            return context.Model.FindEntityType(typeof(T))!;
        }

        [Fact]
        public void ProductConfiguration_ShouldSetTableName()
        {
            var entity = GetEntityType<Product>();
            entity.GetTableName().Should().Be("Products");
        }

        [Fact]
        public void ProductConfiguration_ShouldHaveRequiredFields()
        {
            var entity = GetEntityType<Product>();
            entity.FindProperty(nameof(Product.Name))!.IsNullable.Should().BeFalse();
            entity.FindProperty(nameof(Product.ArName))!.IsNullable.Should().BeFalse();
            entity.FindProperty(nameof(Product.Price))!.IsNullable.Should().BeFalse();
        }

        [Fact]
        public void ProductConfiguration_ShouldHavePrecisionOnPrice()
        {
            var entity = GetEntityType<Product>();
            var priceProp = entity.FindProperty(nameof(Product.Price))!;
            priceProp.GetPrecision().Should().Be(18);
            priceProp.GetScale().Should().Be(2);
        }

        [Fact]
        public void ProductConfiguration_ShouldHaveDefaultValues()
        {
            var entity = GetEntityType<Product>();
            entity.FindProperty(nameof(Product.QuantityInStock))!.GetDefaultValue().Should().Be(0);
            entity.FindProperty(nameof(Product.DangerQuantity))!.GetDefaultValue().Should().Be(0);
            entity.FindProperty(nameof(Product.AverageRating))!.GetDefaultValue().Should().Be(0.0);
            entity.FindProperty(nameof(Product.ReviewCount))!.GetDefaultValue().Should().Be(0);
        }

        [Fact]
        public void CategoryConfiguration_ShouldSetTableName()
        {
            var entity = GetEntityType<Category>();
            entity.GetTableName().Should().Be("Categories");
        }

        [Fact]
        public void CategoryConfiguration_ShouldHaveUniqueIndex()
        {
            var entity = GetEntityType<Category>();
            var index = entity.FindIndex(entity.FindProperty(nameof(Category.CategoryName))!);
            index!.IsUnique.Should().BeTrue();
        }

        [Fact]
        public void ProductConfiguration_ShouldHaveRowVersion()
        {
            var entity = GetEntityType<Product>();
            var rowVersion = entity.FindProperty(nameof(Product.RowVersion))!;
            rowVersion.IsConcurrencyToken.Should().BeTrue();
        }

        [Fact]
        public void AllEntities_ShouldHaveIdAsKey()
        {
            using var context = CreateContext();
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                if (entityType.ClrType.IsSubclassOf(typeof(BaseEntity<Guid>)) ||
                    entityType.ClrType.GetInterfaces().Any(i =>
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(DAKKN.Domain.Common.Interfaces.IBaseEntity<>)))
                {
                    var key = entityType.FindPrimaryKey();
                    key.Should().NotBeNull();
                    key!.Properties.Select(p => p.Name).Should().Contain("Id");
                }
            }
        }

        [Fact]
        public void OrdersConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<Order>();
            entity.GetTableName().Should().Be("Orders");
        }

        [Fact]
        public void OrderItemsConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<OrderItem>();
            entity.GetTableName().Should().Be("OrderItems");
        }

        [Fact]
        public void OrderItemConfiguration_ShouldHaveForeignKeyToOrder()
        {
            var entity = GetEntityType<OrderItem>();
            var fk = entity.GetForeignKeys().FirstOrDefault(f => f.PrincipalEntityType.ClrType == typeof(Order));
            fk.Should().NotBeNull();
        }

        [Fact]
        public void SupportTicketConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<SupportTicket>();
            entity.GetTableName().Should().Be("SupportTickets");
        }

        [Fact]
        public void BrandReviewConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<BrandReview>();
            entity.GetTableName().Should().Be("BrandReviews");
        }

        [Fact]
        public void ShippingGovernorateConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<ShippingGovernorate>();
            entity.GetTableName().Should().Be("ShippingGovernorates");
        }

        [Fact]
        public void UserRefreshTokenConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<UserRefreshToken>();
            entity.GetTableName().Should().Be("UserRefreshTokens");
        }

        [Fact]
        public void UserSettingsConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<UserSettings>();
            entity.GetTableName().Should().Be("UserSettings");
        }

        [Fact]
        public void StickerSuggestionConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<StickerSuggestion>();
            entity.GetTableName().Should().Be("StickerSuggestions");
        }

        [Fact]
        public void LandingPageSettingConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<LandingPageSetting>();
            entity.GetTableName().Should().Be("LandingPageSettings");
        }

        [Fact]
        public void InventoryTransactionConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<InventoryTransaction>();
            entity.GetTableName().Should().Be("InventoryTransactions");
        }

        [Fact]
        public void SystemSettingConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<SystemSetting>();
            entity.GetTableName().Should().Be("SystemSettings");
        }

        [Fact]
        public void UserFavoriteConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<UserFavorite>();
            entity.GetTableName().Should().Be("UserFavorites");
        }

        [Fact]
        public void OrderStatusHistoryConfiguration_ShouldSetTable()
        {
            var entity = GetEntityType<OrderStatusHistory>();
            entity.GetTableName().Should().Be("OrderStatusHistories");
        }
    }
}
