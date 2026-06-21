using System.Text.Json;
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
            var productRepo = _unitOfWork.GetRepository<Product>();
            var settingsRepo = _unitOfWork.GetRepository<LandingPageSetting>();

            var cmsProductsSetting = await settingsRepo.GetBy(s => s.Key == "cms_products").FirstOrDefaultAsync(cancellationToken);
            List<Guid> manualIds = new();

            if (cmsProductsSetting != null && !string.IsNullOrEmpty(cmsProductsSetting.Value))
            {
                try
                {
                    using var doc = JsonDocument.Parse(cmsProductsSetting.Value);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("selectedProductIds", out var ids))
                    {
                        var raw = ids.GetString();
                        if (!string.IsNullOrEmpty(raw))
                        {
                            foreach (var idStr in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (Guid.TryParse(idStr.Trim(), out var gid))
                                    manualIds.Add(gid);
                            }
                        }
                    }
                }
                catch { }
            }

            List<ProductDto> products = new();

            if (manualIds.Count > 0)
            {
                var query = productRepo.GetAllAsync(null).AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => !p.IsDeleted && p.IsActive && manualIds.Contains(p.Id));

                var all = await query
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

                var lookup = all.ToDictionary(p => p.Id);
                products = manualIds
                    .Where(id => lookup.ContainsKey(id))
                    .Select(id => lookup[id])
                    .Take(8)
                    .ToList();
            }

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
