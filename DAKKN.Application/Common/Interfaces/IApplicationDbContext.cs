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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
