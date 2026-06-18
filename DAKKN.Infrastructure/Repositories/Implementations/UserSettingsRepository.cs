using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Infrastructure.Repositories.Implementations
{
    public class UserSettingsRepository : GenericRepository<UserSettings, Guid>, IUserSettingsRepository
    {
        private readonly DAKKNDbContext _dbContext;

        public UserSettingsRepository(DAKKNDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserSettings?> GetByUserIdAsync(Guid userId)
        {
            return await _dbContext.UserSettings
                .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
        }
    }
}
