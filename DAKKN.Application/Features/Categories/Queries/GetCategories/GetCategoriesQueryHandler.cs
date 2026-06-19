using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Category>();
            var query = repo.GetAllAsync(null).AsNoTracking();

            if (!request.IncludeInactive)
            {
                query = query.Where(c => !c.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.CategoryName.ToLower().Contains(term) ||
                    c.ArName.ToLower().Contains(term));
            }

            var categoriesQuery = query
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    ArName = c.ArName,
                    ImageUrl = c.ImageUrl,
                    IsDeleted = c.IsDeleted,
                    ProductsCount = c.Products.Count(p => !p.IsDeleted)
                })
                .OrderByDescending(c => c.ProductsCount);

            if (request.Top.HasValue && request.Top.Value > 0)
            {
                categoriesQuery = (IOrderedQueryable<CategoryDto>)categoriesQuery.Take(request.Top.Value);
            }

            var categories = await categoriesQuery.ToListAsync(cancellationToken);

            foreach (var item in categories)
            {
                if (!string.IsNullOrEmpty(item.ImageUrl))
                {
                    item.ImageFullUrl = item.ImageUrl.StartsWith("http") || item.ImageUrl.StartsWith("/")
                        ? item.ImageUrl
                        : $"/files/{item.ImageUrl}";
                    item.ImageUrl = item.ImageFullUrl;
                }
            }

            return categories;
        }
    }
}
