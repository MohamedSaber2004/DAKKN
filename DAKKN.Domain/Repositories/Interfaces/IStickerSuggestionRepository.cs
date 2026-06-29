using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;

namespace DAKKN.Domain.Repositories.Interfaces
{
    public interface IStickerSuggestionRepository : IGenericRepository<StickerSuggestion, Guid>
    {
        IQueryable<StickerSuggestion> GetByUserIdAsync(Guid userId);
    }
}
