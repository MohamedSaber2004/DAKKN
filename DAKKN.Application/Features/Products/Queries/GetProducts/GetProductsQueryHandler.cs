using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Models;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagginatedResult<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagginatedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var query = repo.GetAllAsync(null).AsNoTracking()
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(term) || p.Description.ToLower().Contains(term));
            }

            if (request.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }

            var projected = query.Select(p => new ProductDto
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
                IsDeleted = p.IsDeleted
            });

            var result = await projected.AsPagginatedListAsync(request.PageNumber, request.PageSize);

            foreach (var item in result.Items)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{item.ImageUrl}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return result;
        }
    }
}
