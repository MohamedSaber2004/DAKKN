namespace DAKKN.Application.Features.ProductRatings.DTOs
{
    public class ProductRatingSummaryDto
    {
        public Guid ProductId { get; set; }
        public double AverageStars { get; set; }
        public int TotalRatings { get; set; }
        public int? CurrentUserStars { get; set; }
    }
}
