using DAKKN.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DAKKN.Persistence
{
    public static class DAKKNDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            // Seed Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpper() });
                }
            }

            // Seed Admin User
            var adminEmail = "dev.mohamed104saber@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser(adminEmail, adminEmail, "System Admin", new DateTime(1990, 1, 1), Domain.Enums.Gender.Male)
                {
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    IsActive = true
                };

                await userManager.CreateAsync(adminUser, "Admin@123");
            }

            // Ensure Admin role is assigned
            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        public static async Task SeedCategoriesAndProductsAsync(DAKKNDbContext context)
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "DAKKN.Persistence", "SeedData");

            if (!context.Categories.Any())
            {
                var json = await File.ReadAllTextAsync(Path.Combine(basePath, "categories.json"));
                var items = JsonSerializer.Deserialize<List<CategorySeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (items != null)
                {
                    foreach (var item in items)
                        context.Categories.Add(new Category { CategoryName = item.CategoryName, ArName = item.ArName, ImageUrl = item.ImageUrl });
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Products.Any())
            {
                var json = await File.ReadAllTextAsync(Path.Combine(basePath, "products.json"));
                var items = JsonSerializer.Deserialize<List<ProductSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (items != null)
                {
                    var catMap = context.Categories.ToDictionary(c => c.CategoryName, c => c.Id);
                    foreach (var item in items)
                    {
                        if (catMap.TryGetValue(item.CategoryName, out var catId))
                        {
                            context.Products.Add(new Product
                            {
                                Name = item.Name,
                                ArName = item.ArName,
                                Description = item.Description,
                                ArDescription = item.ArDescription,
                                Price = item.Price,
                                AverageRating = item.AverageRating,
                                ReviewCount = item.ReviewCount,
                                ImageUrl = item.ImageUrl,
                                FinishOptions = item.FinishOptions ?? new(),
                                SizeOptions = item.SizeOptions ?? new(),
                                CategoryId = catId
                            });
                        }
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        private class CategorySeed
        {
            public string CategoryName { get; set; } = string.Empty;
            public string ArName { get; set; } = string.Empty;
            public string? ImageUrl { get; set; }
        }

        private class ProductSeed
        {
            public string Name { get; set; } = string.Empty;
            public string ArName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ArDescription { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public double AverageRating { get; set; }
            public int ReviewCount { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
            public string CategoryName { get; set; } = string.Empty;
            public List<string>? FinishOptions { get; set; }
            public List<string>? SizeOptions { get; set; }
        }
    }
}
