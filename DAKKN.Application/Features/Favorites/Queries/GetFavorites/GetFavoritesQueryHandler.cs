using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Favorites.Queries.GetFavorites
{
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetFavoritesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<ProductDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<UserFavorite>();
            var favorites = await repo.GetAllAsync(f => f.UserId == _currentUserService.UserId && !f.IsDeleted && !f.Product.IsDeleted).AsNoTracking()
                .Include(f => f.Product)
                    .ThenInclude(p => p.Category)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);

            var result = favorites.Select(f => new ProductDto
            {
                Id = f.Product.Id,
                Name = f.Product.Name,
                ArName = f.Product.ArName,
                Description = f.Product.Description,
                ArDescription = f.Product.ArDescription,
                Price = f.Product.Price,
                AverageRating = f.Product.AverageRating,
                ReviewCount = f.Product.ReviewCount,
                ImageUrl = f.Product.ImageUrl,
                FinishOptions = f.Product.FinishOptions,
                SizeOptions = f.Product.SizeOptions,
                CategoryId = f.Product.CategoryId,
                CategoryName = f.Product.Category.CategoryName,
                CategoryArName = f.Product.Category.ArName,
                CreatedAt = f.Product.CreatedAt,
                UpdatedAt = f.Product.UpdatedAt,
                IsActive = f.Product.IsActive,
                IsDeleted = f.Product.IsDeleted,
                QuantityInStock = f.Product.QuantityInStock,
                DangerQuantity = f.Product.DangerQuantity,
                StockStatus = f.Product.StockStatus.ToString(),
                IsInStock = f.Product.IsInStock
            }).ToList();

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{Path.GetFileName(item.ImageUrl)}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return result;
        }
    }
}
