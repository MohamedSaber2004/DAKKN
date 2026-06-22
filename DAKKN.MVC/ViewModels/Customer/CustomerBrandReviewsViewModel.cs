using DAKKN.Application.Features.BrandReviews.DTOs;

namespace DAKKN.MVC.ViewModels.Customer
{
    public class CustomerBrandReviewsViewModel
    {
        public List<BrandReviewDto> Reviews { get; set; } = new();
    }
}
