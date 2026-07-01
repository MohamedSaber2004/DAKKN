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
            var baseQuery = _context.Products.Where(p => !p.IsDeleted);

            var stats = await baseQuery
                .GroupBy(p => 1)
                .Select(g => new
                {
                    TotalProducts = g.Count(),
                    LowStockCount = g.Count(p => p.QuantityInStock > 0 && p.QuantityInStock <= p.DangerQuantity),
                    OutOfStockCount = g.Count(p => p.QuantityInStock == 0)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (stats is null)
            {
                return new DashboardInventoryStatsDto();
            }

            return new DashboardInventoryStatsDto
            {
                TotalProducts = stats.TotalProducts,
                LowStockCount = stats.LowStockCount,
                OutOfStockCount = stats.OutOfStockCount
            };
        }
    }
}
