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
        DbSet<UserFavorite> UserFavorites { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
