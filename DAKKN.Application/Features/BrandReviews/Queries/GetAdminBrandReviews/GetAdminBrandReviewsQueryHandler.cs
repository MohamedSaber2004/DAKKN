using DAKKN.Application.Features.BrandReviews.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.BrandReviews.Queries.GetAdminBrandReviews
{
    public class GetAdminBrandReviewsQueryHandler : IRequestHandler<GetAdminBrandReviewsQuery, List<BrandReviewDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAdminBrandReviewsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<BrandReviewDto>> Handle(GetAdminBrandReviewsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<BrandReview>();
            var query = repo.GetAllAsync(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.StatusFilter))
            {
                query = request.StatusFilter.ToLower() switch
                {
                    "pending" => query.Where(r => !r.IsApproved),
                    "approved" => query.Where(r => r.IsApproved && !r.IsDisplayed),
                    "displayed" => query.Where(r => r.IsDisplayed),
                    "rejected" => query.Where(r => !r.IsApproved && r.ApprovedBy != null),
                    _ => query
                };
            }

            if (request.RatingFilter.HasValue)
                query = query.Where(r => r.Rating == request.RatingFilter.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(r => r.ReviewTitle.ToLower().Contains(term) || r.ReviewText.ToLower().Contains(term));
            }

            query = (request.SortBy?.ToLower()) switch
            {
                "oldest" => query.OrderBy(r => r.CreatedAt),
                "rating_desc" => query.OrderByDescending(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                "rating_asc" => query.OrderBy(r => r.Rating).ThenByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            var reviews = await query
                .Include(r => r.User)
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
