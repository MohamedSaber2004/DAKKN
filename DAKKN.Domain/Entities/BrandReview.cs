using DAKKN.Domain.Common;

namespace DAKKN.Domain.Entities
{
    public class BrandReview : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public int Rating { get; set; }
        public string ReviewTitle { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;

        public bool IsApproved { get; set; }
        public bool IsDisplayed { get; set; }
        public int DisplayOrder { get; set; }

        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
