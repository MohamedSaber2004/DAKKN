using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DAKKN.Persistence
{
    public class DAKKNDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid,
        IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IApplicationDbContext
    {
        public DAKKNDbContext(DbContextOptions<DAKKNDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserSettings> UserSettings => Set<UserSettings>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ShippingGovernorate> ShippingGovernorates => Set<ShippingGovernorate>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
        public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.ConfigureWarnings(action =>
            {
                action.Ignore(CoreEventId.InvalidIncludePathError);
            });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(DAKKNDbContext).Assembly,
                type => type.Namespace is not null && (type.Namespace.EndsWith("Configurations") || type.Namespace.EndsWith("Configuration")));

            builder.HasDefaultSchema("dbo");
        }
    }
}
