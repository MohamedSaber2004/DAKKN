using DAKKN.Application.Features.BrandReviews.DTOs;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class AdminBrandReviewsViewModel
    {
        public List<BrandReviewDto> Reviews { get; set; } = new();
        public string? StatusFilter { get; set; }
        public int? RatingFilter { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }
}
