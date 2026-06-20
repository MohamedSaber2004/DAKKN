using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.ShippingGovernorates.Queries.GetActiveShippingGovernorates
{
    public class GetActiveShippingGovernoratesQueryHandler : IRequestHandler<GetActiveShippingGovernoratesQuery, List<ShippingGovernorateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveShippingGovernoratesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ShippingGovernorateDto>> Handle(GetActiveShippingGovernoratesQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<ShippingGovernorate>();
            return await repo.GetAllAsync(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name)
                .Select(x => new ShippingGovernorateDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    ArName = x.ArName,
                    ShippingPrice = x.ShippingPrice,
                    IsActive = true,
                    DisplayOrder = x.DisplayOrder
                })
                .ToListAsync(cancellationToken);
        }
    }
}
