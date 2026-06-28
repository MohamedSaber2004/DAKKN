using System;

namespace DAKKN.Application.Features.Dashboard.Queries.GetRecentProductRatings
{
    public class RecentProductRatingDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public string? ProductImageFullUrl { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerImageUrl { get; set; }
        public string? CustomerImageFullUrl { get; set; }
        public int Stars { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
