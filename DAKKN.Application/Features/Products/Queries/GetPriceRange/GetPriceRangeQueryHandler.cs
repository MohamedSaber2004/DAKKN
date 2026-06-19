using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Products.Queries.GetPriceRange
{
    public class GetPriceRangeQueryHandler : IRequestHandler<GetPriceRangeQuery, PriceRangeDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPriceRangeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PriceRangeDto> Handle(GetPriceRangeQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();
            var query = repo.GetAllAsync(null).AsNoTracking().Where(p => !p.IsDeleted);

            var min = await query.MinAsync(p => (decimal?)p.Price, cancellationToken) ?? 0;
            var max = await query.MaxAsync(p => (decimal?)p.Price, cancellationToken) ?? 0;

            return new PriceRangeDto
            {
                MinPrice = Math.Floor(min),
                MaxPrice = Math.Ceiling(max)
            };
        }
    }
}
