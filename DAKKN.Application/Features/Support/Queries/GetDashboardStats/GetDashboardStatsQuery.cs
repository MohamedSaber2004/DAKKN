using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery : IRequest<SupportDashboardStatsDto>;
}
