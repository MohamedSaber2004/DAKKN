using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetCustomerBrandReviews
{
    public class GetCustomerBrandReviewsQueryHandler : IRequestHandler<GetCustomerBrandReviewsQuery, List<BrandReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomerBrandReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BrandReviewDto>> Handle(GetCustomerBrandReviewsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var reviews = await repo.GetAllAsync(r => r.UserId == request.UserId && !r.IsDeleted)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            return reviews.Select(r => new BrandReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                CustomerName = r.User?.FullName ?? string.Empty,
                CustomerEmail = r.User?.Email ?? string.Empty,
                ProfilePictureUrl = r.User?.ProfilePictureUrl,
                Rating = r.Rating,
                ReviewTitle = r.ReviewTitle,
                ReviewText = r.ReviewText,
                IsApproved = r.IsApproved,
                IsDisplayed = r.IsDisplayed,
                DisplayOrder = r.DisplayOrder,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                ApprovedAt = r.ApprovedAt
            }).ToList();
        }
    }
}
