using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Products.Queries.GetFeaturedProducts
{
    public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFeaturedProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();

            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var query = repo.GetAllAsync(null).AsNoTracking()
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.IsActive && p.CreatedAt >= sevenDaysAgo)
                .OrderByDescending(p => p.CreatedAt)
                .Take(8);

            var products = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ArName = p.ArName,
                    Description = p.Description,
                    ArDescription = p.ArDescription,
                    Price = p.Price,
                    AverageRating = p.AverageRating,
                    ReviewCount = p.ReviewCount,
                    ImageUrl = p.ImageUrl,
                    FinishOptions = p.FinishOptions,
                    SizeOptions = p.SizeOptions,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    CategoryArName = p.Category.ArName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsActive = p.IsActive,
                    IsDeleted = p.IsDeleted,
                    QuantityInStock = p.QuantityInStock,
                    DangerQuantity = p.DangerQuantity,
                    StockStatus = p.StockStatus.ToString(),
                    IsInStock = p.IsInStock
                })
                .ToListAsync(cancellationToken);

            foreach (var item in products)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{item.ImageUrl}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return products;
        }
    }
}
