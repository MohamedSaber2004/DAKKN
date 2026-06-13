using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;

namespace DAKKN.Infrastructure.Repositories.Implementations
{
    public class UserRefreshTokenRepository : GenericRepository<UserRefreshToken, Guid>, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(DAKKNDbContext context) : base(context)
        {
        }
    }
}
