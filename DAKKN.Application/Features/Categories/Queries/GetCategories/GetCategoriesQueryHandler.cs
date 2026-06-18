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
            var query = repo.GetAllAsync(null);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.CategoryName.ToLower().Contains(term) ||
                    c.ArName.ToLower().Contains(term));
            }

            var categories = await query
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    ArName = c.ArName,
                    IsDeleted = c.IsDeleted,
                    ProductsCount = c.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync(cancellationToken);

            return categories;
        }
    }
}
