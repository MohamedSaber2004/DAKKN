using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetDisplayedBrandReviews
{
    public class GetDisplayedBrandReviewsQueryHandler : IRequestHandler<GetDisplayedBrandReviewsQuery, List<BrandReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDisplayedBrandReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BrandReviewDto>> Handle(GetDisplayedBrandReviewsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var reviews = await repo.GetAllAsync(r => r.IsDisplayed && r.IsApproved && !r.IsDeleted)
                .Include(r => r.User)
                .OrderBy(r => r.DisplayOrder)
                .ThenByDescending(r => r.CreatedAt)
                .Take(3)
                .ToListAsync(cancellationToken);

            var result = reviews.Select(r => new BrandReviewDto
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

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.ProfilePictureUrl))
                {
                    item.ProfilePictureUrl = $"/files/{Path.GetFileName(item.ProfilePictureUrl)}";
                }
            }

            return result;
        }
    }
}
