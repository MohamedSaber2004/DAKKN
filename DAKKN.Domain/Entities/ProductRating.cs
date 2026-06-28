using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class ProductRating : BaseEntity<Guid>
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public int Stars { get; set; }
    }
}
