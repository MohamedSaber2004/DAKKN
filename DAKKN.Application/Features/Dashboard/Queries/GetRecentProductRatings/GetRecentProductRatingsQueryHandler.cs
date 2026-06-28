using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Dashboard.Queries.GetRecentProductRatings
{
    public class GetRecentProductRatingsQueryHandler : IRequestHandler<GetRecentProductRatingsQuery, List<RecentProductRatingDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRecentProductRatingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<RecentProductRatingDto>> Handle(GetRecentProductRatingsQuery request, CancellationToken cancellationToken)
        {
            var ratingRepo = _unitOfWork.GetRepository<ProductRating>();

            var ratings = await ratingRepo.GetAllAsync(null)
                .AsNoTracking()
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => !r.IsDeleted)
                .OrderByDescending(r => r.CreatedAt)
                .Take(request.Count)
                .Select(r => new RecentProductRatingDto
                {
                    ProductId = r.ProductId,
                    ProductName = r.Product != null ? r.Product.Name : string.Empty,
                    ProductImageUrl = r.Product != null ? r.Product.ImageUrl : string.Empty,
                    UserId = r.UserId,
                    CustomerName = r.User != null ? r.User.FullName : string.Empty,
                    CustomerImageUrl = r.User != null ? r.User.ProfilePictureUrl : null,
                    Stars = r.Stars,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync(cancellationToken);

            foreach (var r in ratings)
            {
                if (!string.IsNullOrEmpty(r.ProductImageUrl))
                {
                    r.ProductImageFullUrl = r.ProductImageUrl.StartsWith("http") || r.ProductImageUrl.StartsWith("/")
                        ? r.ProductImageUrl
                        : $"/files/{Path.GetFileName(r.ProductImageUrl)}";
                }
                if (!string.IsNullOrEmpty(r.CustomerImageUrl))
                {
                    r.CustomerImageFullUrl = r.CustomerImageUrl.StartsWith("http") || r.CustomerImageUrl.StartsWith("/")
                        ? r.CustomerImageUrl
                        : $"/files/{Path.GetFileName(r.CustomerImageUrl)}";
                }
            }

            return ratings;
        }
    }
}
