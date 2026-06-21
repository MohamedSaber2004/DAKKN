using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class UserFavorite : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
