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

            var baseQuery = productRepo.GetAllAsync(null).AsNoTracking()
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.IsActive);

            // 1. Primary: Load all products explicitly marked as Featured by the Admin in DB
            var featuredDbProducts = await baseQuery
                .Where(p => p.IsFeatured)
                .ToListAsync(cancellationToken);

            List<ProductDto> products = new();

            if (featuredDbProducts.Count > 0)
            {
                // Optional ordering from CMS manual sequence if configured
                var cmsProductsSetting = await settingsRepo.GetBy(s => s.Key == "cms_products").FirstOrDefaultAsync(cancellationToken);
                List<Guid> manualIds = new();
                if (cmsProductsSetting != null && !string.IsNullOrEmpty(cmsProductsSetting.Value))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(cmsProductsSetting.Value);
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var prop in doc.RootElement.EnumerateObject())
                            {
                                if (string.Equals(prop.Name, "selectedProductIds", StringComparison.OrdinalIgnoreCase))
                                {
                                    var raw = prop.Value.GetString();
                                    if (!string.IsNullOrEmpty(raw))
                                    {
                                        foreach (var idStr in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            if (Guid.TryParse(idStr.Trim(), out var gid))
                                                manualIds.Add(gid);
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch { }
                }

                var featuredLookup = featuredDbProducts.ToDictionary(p => p.Id);

                // Add products matching manualIds in order (only if they are actually featured)
                foreach (var id in manualIds)
                {
                    if (featuredLookup.TryGetValue(id, out var p))
                    {
                        products.Add(MapToDto(p));
                        featuredLookup.Remove(id);
                    }
                }

                // Append any remaining featured products (ordered by rating then date)
                var remaining = featuredLookup.Values
                    .OrderByDescending(p => p.AverageRating)
                    .ThenByDescending(p => p.CreatedAt)
                    .Select(MapToDto);

                products.AddRange(remaining);

                // Take up to 8 products for the landing page featured showcase
                products = products.Take(8).ToList();
            }

            return products;
        }

        private static ProductDto MapToDto(Product p)
        {
            var resolvedUrl = string.IsNullOrEmpty(p.ImageUrl) ? string.Empty
                : p.ImageUrl.StartsWith("http") || p.ImageUrl.StartsWith("/")
                    ? p.ImageUrl
                    : $"/files/{Path.GetFileName(p.ImageUrl)}";

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                ArName = p.ArName,
                Description = p.Description,
                ArDescription = p.ArDescription,
                Price = p.Price,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                ImageUrl = resolvedUrl,
                ImageFullUrl = resolvedUrl,
                FinishOptions = p.FinishOptions,
                SizeOptions = p.SizeOptions,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName ?? string.Empty,
                CategoryArName = p.Category?.ArName ?? string.Empty,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsActive = p.IsActive,
                IsDeleted = p.IsDeleted,
                QuantityInStock = p.QuantityInStock,
                DangerQuantity = p.DangerQuantity,
                StockStatus = p.StockStatus.ToString(),
                IsInStock = p.IsInStock,
                IsFeatured = p.IsFeatured
            };
        }
    }
}
