using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetFAQCategories
{
    public class GetFAQCategoriesQueryHandler : IRequestHandler<GetFAQCategoriesQuery, List<SupportFAQCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFAQCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupportFAQCategoryDto>> Handle(GetFAQCategoriesQuery request, CancellationToken ct)
        {
            return await _unitOfWork.GetRepository<SupportFAQCategory>()
                .GetAllAsync(c => c.IsActive)
                .Include(c => c.FAQs)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new SupportFAQCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ArName = c.ArName,
                    Icon = c.Icon,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive,
                    FAQCount = c.FAQs.Count(f => f.IsPublished)
                }).ToListAsync(ct);
        }
    }
}
