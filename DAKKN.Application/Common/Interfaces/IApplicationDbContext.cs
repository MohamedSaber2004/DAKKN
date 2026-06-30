using DAKKN.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<IdentityRole<Guid>> Roles { get; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }
        DbSet<UserSettings> UserSettings { get; }
        DbSet<Product> Products { get; }
        DbSet<Category> Categories { get; }
        DbSet<ShippingGovernorate> ShippingGovernorates { get; }
        DbSet<SystemSetting> SystemSettings { get; }
        DbSet<LandingPageSetting> LandingPageSettings { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DbSet<OrderStatusHistory> OrderStatusHistories { get; }
        DbSet<UserFavorite> UserFavorites { get; }
        DbSet<StickerSuggestion> StickerSuggestions { get; }
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<SupportReply> SupportReplies { get; }
        DbSet<SupportAttachment> SupportAttachments { get; }
        DbSet<SupportCategory> SupportCategories { get; }
        DbSet<SupportFAQ> SupportFAQs { get; }
        DbSet<SupportFAQCategory> SupportFAQCategories { get; }
        DbSet<SupportActivity> SupportActivities { get; }
        DbSet<SupportInternalNote> SupportInternalNotes { get; }
        DbSet<SupportSettings> SupportSettings { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
