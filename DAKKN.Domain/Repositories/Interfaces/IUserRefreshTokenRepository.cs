using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;

namespace DAKKN.Domain.Repositories.Interfaces
{
    public interface IUserRefreshTokenRepository : IGenericRepository<UserRefreshToken, Guid>
    {
    }
}
