using DAKKN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Attachment> Attachments { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
