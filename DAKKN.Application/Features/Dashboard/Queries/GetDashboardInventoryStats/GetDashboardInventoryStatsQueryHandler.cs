using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Dashboard.Queries.GetDashboardInventoryStats
{
    public class GetDashboardInventoryStatsQueryHandler : IRequestHandler<GetDashboardInventoryStatsQuery, DashboardInventoryStatsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetDashboardInventoryStatsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardInventoryStatsDto> Handle(GetDashboardInventoryStatsQuery request, CancellationToken cancellationToken)
        {
            var products = await _context.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync(cancellationToken);

            return new DashboardInventoryStatsDto
            {
                TotalProducts = products.Count,
                LowStockCount = products.Count(p => p.QuantityInStock > 0 && p.QuantityInStock <= p.DangerQuantity),
                OutOfStockCount = products.Count(p => p.QuantityInStock == 0)
            };
        }
    }
}
