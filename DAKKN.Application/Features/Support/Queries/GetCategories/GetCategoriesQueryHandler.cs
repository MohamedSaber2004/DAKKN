using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<SupportCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SupportCategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportCategory>()
                .GetAllAsync(_ => true)
                .AsQueryable();

            if (!request.IncludeInactive)
                query = query.Where(c => c.IsActive);

            return await query.OrderBy(c => c.DisplayOrder).Select(c => new SupportCategoryDto
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
