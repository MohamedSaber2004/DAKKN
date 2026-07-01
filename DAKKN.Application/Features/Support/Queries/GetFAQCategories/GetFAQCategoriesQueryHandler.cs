using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetFAQCategories
{
    public class GetFAQCategoriesQueryHandler : IRequestHandler<GetFAQCategoriesQuery, List<SupportCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFAQCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupportCategoryDto>> Handle(GetFAQCategoriesQuery request, CancellationToken ct)
        {
            return await _unitOfWork.GetRepository<SupportCategory>()
                .GetAllAsync(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new SupportCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ArName = c.ArName,
                    Description = c.Description,
                    Icon = c.Icon,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive
                }).ToListAsync(ct);
        }
    }
}
