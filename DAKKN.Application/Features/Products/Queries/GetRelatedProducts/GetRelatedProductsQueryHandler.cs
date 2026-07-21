using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Products.Queries.GetRelatedProducts
{
    public class GetRelatedProductsQueryHandler : IRequestHandler<GetRelatedProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRelatedProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> Handle(GetRelatedProductsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var query = repo.GetAllAsync(null).AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == request.CategoryId
                         && p.Id != request.ProductId
                         && !p.IsDeleted
                         && p.IsActive);

            var related = await query
                .OrderBy(_ => EF.Functions.Random())
                .Take(5)
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

            foreach (var item in related)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{Path.GetFileName(item.ImageUrl)}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return related;
        }
    }
}
