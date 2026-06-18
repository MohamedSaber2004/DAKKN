using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;

namespace DAKKN.Domain.Repositories.Interfaces
{
    public interface IUserSettingsRepository : IGenericRepository<UserSettings, Guid>
    {
        Task<UserSettings?> GetByUserIdAsync(Guid userId);
    }
}
