using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Products.Queries.GetMostOrderedProducts
{
    public class GetMostOrderedProductsQueryHandler : IRequestHandler<GetMostOrderedProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMostOrderedProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProductDto>> Handle(GetMostOrderedProductsQuery request, CancellationToken cancellationToken)
        {
            var orderItemRepo = _unitOfWork.GetRepository<OrderItem>();
            var productRepo = _unitOfWork.GetRepository<Product>();

            var topProductIds = await orderItemRepo.GetAllAsync(null)
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, OrderCount = g.Count() })
                .OrderByDescending(x => x.OrderCount)
                .Take(request.Count)
                .ToListAsync(cancellationToken);

            if (topProductIds.Count == 0)
                return new List<ProductDto>();

            var ids = topProductIds.Select(x => x.ProductId).ToList();

            var products = await productRepo.GetAllAsync(null)
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Ratings.Where(r => !r.IsDeleted))
                .Where(p => ids.Contains(p.Id) && !p.IsDeleted)
                .ToListAsync(cancellationToken);

            var ordered = ids
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .Select(p => new ProductDto
                {
                    Id = p!.Id,
                    Name = p.Name,
                    ArName = p.ArName,
                    Description = p.Description,
                    ArDescription = p.ArDescription,
                    Price = p.Price,
                    AverageRating = p.Ratings.Count > 0 ? Math.Round(p.Ratings.Average(r => r.Stars), 1) : 0,
                    ReviewCount = p.Ratings.Count,
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
                .ToList();

            foreach (var item in ordered)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{Path.GetFileName(item.ImageUrl)}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return ordered;
        }
    }
}
