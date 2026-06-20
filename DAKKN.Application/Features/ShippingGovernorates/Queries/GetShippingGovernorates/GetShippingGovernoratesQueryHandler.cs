using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.ShippingGovernorates.Queries.GetShippingGovernorates
{
    public class GetShippingGovernoratesQueryHandler : IRequestHandler<GetShippingGovernoratesQuery, List<ShippingGovernorateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetShippingGovernoratesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ShippingGovernorateDto>> Handle(GetShippingGovernoratesQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            var query = repo.GetAllAsync(null);

            if (!request.IncludeInactive)
                query = query.Where(x => x.IsActive && !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term) || x.ArName.Contains(term));
            }

            return await query
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .Select(x => new ShippingGovernorateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ArName = x.ArName,
                    ShippingPrice = x.ShippingPrice,
                    IsActive = x.IsActive && !x.IsDeleted,
                    DisplayOrder = x.DisplayOrder
                })
                .ToListAsync(cancellationToken);
        }
    }
}
