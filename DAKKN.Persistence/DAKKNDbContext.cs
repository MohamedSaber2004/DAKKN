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
        public DbSet<LandingPageSetting> LandingPageSettings => Set<LandingPageSetting>();
        public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<BrandReview> BrandReviews => Set<BrandReview>();
        public DbSet<StickerSuggestion> StickerSuggestions => Set<StickerSuggestion>();
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<SupportReply> SupportReplies => Set<SupportReply>();
        public DbSet<SupportAttachment> SupportAttachments => Set<SupportAttachment>();
        public DbSet<SupportCategory> SupportCategories => Set<SupportCategory>();
        public DbSet<SupportFAQ> SupportFAQs => Set<SupportFAQ>();
        public DbSet<SupportFAQCategory> SupportFAQCategories => Set<SupportFAQCategory>();
        public DbSet<SupportActivity> SupportActivities => Set<SupportActivity>();
        public DbSet<SupportInternalNote> SupportInternalNotes => Set<SupportInternalNote>();
        public DbSet<SupportSettings> SupportSettings => Set<SupportSettings>();

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
