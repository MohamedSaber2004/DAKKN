using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Infrastructure.Repositories.Implementations.Base;
using DAKKN.Persistence;

namespace DAKKN.Infrastructure.Repositories.Implementations
{
    public class StickerSuggestionRepository : GenericRepository<StickerSuggestion, Guid>, IStickerSuggestionRepository
    {
        public StickerSuggestionRepository(DAKKNDbContext context) : base(context)
        {
        }

        public IQueryable<StickerSuggestion> GetByUserIdAsync(Guid userId)
        {
            return GetAllAsync(s => s.UserId == userId);
        }
    }
}
