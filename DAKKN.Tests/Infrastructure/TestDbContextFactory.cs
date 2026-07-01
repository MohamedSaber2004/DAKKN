using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Tests.Infrastructure
{
    public static class TestDbContextFactory
    {
        public static DAKKNDbContext Create()
        {
            var options = new DbContextOptionsBuilder<DAKKNDbContext>()
                .UseInMemoryDatabase(databaseName: $"DAKKN_Test_{Guid.NewGuid()}")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.InvalidIncludePathError))
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var context = new DAKKNDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static DAKKNDbContext CreateWithSeed()
        {
            var context = Create();

            var category = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "TestCategory",
                ArName = "تصنيف اختبار",
                IsDeleted = false
            };
            context.Categories.Add(category);

            var products = new List<Product>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Product A",
                    ArName = "منتج أ",
                    Description = "Description A",
                    ArDescription = "وصف أ",
                    Price = 100m,
                    QuantityInStock = 10,
                    DangerQuantity = 3,
                    CategoryId = category.Id,
                    IsDeleted = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Product B",
                    ArName = "منتج ب",
                    Description = "Description B",
                    ArDescription = "وصف ب",
                    Price = 200m,
                    QuantityInStock = 2,
                    DangerQuantity = 5,
                    CategoryId = category.Id,
                    IsDeleted = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Product C",
                    ArName = "منتج ج",
                    Description = "Description C",
                    ArDescription = "وصف ج",
                    Price = 50m,
                    QuantityInStock = 0,
                    DangerQuantity = 2,
                    CategoryId = category.Id,
                    IsDeleted = true
                }
            };
            context.Products.AddRange(products);

            var category2 = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "Electronics",
                ArName = "إلكترونيات",
                IsDeleted = false
            };
            var category3 = new Category
            {
                Id = Guid.NewGuid(),
                CategoryName = "DeletedCat",
                ArName = "محذوف",
                IsDeleted = true
            };
            context.Categories.AddRange(category2, category3);

            context.SaveChanges();

            foreach (var p in context.Products.ToList())
            {
                context.Entry(p).State = EntityState.Detached;
            }
            foreach (var c in context.Categories.ToList())
            {
                context.Entry(c).State = EntityState.Detached;
            }

            return context;
        }
    }
}
