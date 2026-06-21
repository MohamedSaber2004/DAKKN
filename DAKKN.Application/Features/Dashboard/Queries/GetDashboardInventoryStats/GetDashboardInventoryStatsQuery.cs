using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Dashboard.Queries.GetDashboardInventoryStats
{
    public record GetDashboardInventoryStatsQuery : IRequest<DashboardInventoryStatsDto>;
}
