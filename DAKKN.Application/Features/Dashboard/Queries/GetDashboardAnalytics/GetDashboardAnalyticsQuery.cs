using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Dashboard.Queries.GetDashboardAnalytics
{
    public record GetDashboardAnalyticsQuery(int Days = 7) : IRequest<DashboardAnalyticsDto>;
}
