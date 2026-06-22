namespace DAKKN.Application.Features.BrandReviews.DTOs
{
    public class BrandReviewDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public int Rating { get; set; }
        public string ReviewTitle { get; set; } = string.Empty;
        public string ReviewText { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public bool IsDisplayed { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}
