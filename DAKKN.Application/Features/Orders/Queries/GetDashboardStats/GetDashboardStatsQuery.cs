using DAKKN.Application.Features.Orders.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Orders.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;
}
