using DAKKN.Domain.Common;
using DAKKN.Domain.Enums;

namespace DAKKN.Domain.Entities
{
    public sealed class StickerSuggestion : BaseEntity<Guid>
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? ReferenceImagePath { get; private set; }
        public string? Tags { get; private set; }
        public SuggestionStatus Status { get; private set; }
        public string? AdminNote { get; private set; }
        public Guid? ConvertedProductId { get; private set; }

        public ApplicationUser User { get; private set; } = null!;
        public Product? Product { get; private set; }

        private StickerSuggestion() { }

        public static StickerSuggestion Create(Guid userId, string title,
            string description, string? imagePath, string? tags)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));

            return new StickerSuggestion
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title.Trim(),
                Description = description.Trim(),
                ReferenceImagePath = imagePath,
                Tags = tags?.Trim(),
                Status = SuggestionStatus.Pending
            };
        }

        public void MarkUnderReview(Guid adminId)
        {
            Status = SuggestionStatus.UnderReview;
            MarkAsUpdated(adminId.ToString());
        }

        public void Approve(string? adminNote, Guid adminId)
        {
            Status = SuggestionStatus.Approved;
            AdminNote = adminNote;
            MarkAsUpdated(adminId.ToString());
        }

        public void Reject(string? adminNote, Guid adminId)
        {
            Status = SuggestionStatus.Rejected;
            AdminNote = adminNote;
            MarkAsUpdated(adminId.ToString());
        }

        public void LinkProduct(Guid productId, Guid adminId)
        {
            ConvertedProductId = productId;
            MarkAsUpdated(adminId.ToString());
        }
    }
}
