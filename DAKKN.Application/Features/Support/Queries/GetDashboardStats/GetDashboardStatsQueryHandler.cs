using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, SupportDashboardStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportDashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken ct)
        {
            var tickets = await _unitOfWork.GetRepository<SupportTicket>()
                .GetAllAsync(t => !t.IsDeleted)
                .Include(t => t.Category)
                .ToListAsync(ct);

            var now = DateTime.UtcNow;
            var stats = new SupportDashboardStatsDto
            {
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == SupportTicketStatus.Open),
                PendingTickets = tickets.Count(t => t.Status == SupportTicketStatus.InProgress),
                WaitingCustomer = tickets.Count(t => t.Status == SupportTicketStatus.WaitingCustomer),
                ClosedTickets = tickets.Count(t => t.Status == SupportTicketStatus.Closed),
                HighPriorityTickets = tickets.Count(t => t.Priority >= SupportTicketPriority.High),
                AverageResponseTime = CalculateAverage(tickets.Where(t => t.FirstResponseAt != null)
                    .Select(t => (t.FirstResponseAt!.Value - t.CreatedAt).TotalHours)),
                AverageResolutionTime = CalculateAverage(tickets.Where(t => t.ResolvedAt != null)
                    .Select(t => (t.ResolvedAt!.Value - t.CreatedAt).TotalHours))
            };

            stats.TicketsByDay = tickets.GroupBy(t => t.CreatedAt.Date)
                .Select(g => new ChartDataPoint { Label = g.Key.ToString("MMM dd"), Value = g.Count() })
                .ToList();

            stats.TicketsByWeek = tickets.GroupBy(t => GetWeekNumber(t.CreatedAt))
                .Select(g => new ChartDataPoint { Label = $"Week {g.Key}", Value = g.Count() })
                .ToList();

            stats.TicketsByMonth = tickets.GroupBy(t => t.CreatedAt.ToString("yyyy-MM"))
                .Select(g => new ChartDataPoint { Label = g.Key, Value = g.Count() })
                .ToList();

            stats.PriorityDistribution = tickets.GroupBy(t => t.Priority)
                .Select(g => new ChartDataPoint { Label = g.Key.ToString(), Value = g.Count() })
                .ToList();

            stats.CategoryDistribution = tickets.GroupBy(t => t.Category.Name)
                .Select(g => new ChartDataPoint { Label = g.Key, Value = g.Count() })
                .ToList();

            stats.StatusDistribution = tickets.GroupBy(t => t.Status)
                .Select(g => new ChartDataPoint { Label = g.Key.ToString(), Value = g.Count() })
                .ToList();

            stats.RecentTickets = tickets.OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .Select(t => new SupportDashboardRecentTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    Subject = t.Subject,
                    CustomerName = t.CustomerName,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt
                })
                .ToList();

            stats.TopIssues = tickets.GroupBy(t => new { t.Subject, t.Category.Name })
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new TopIssueDto
                {
                    Subject = g.Key.Subject,
                    Count = g.Count(),
                    CategoryName = g.Key.Name
                })
                .ToList();

            return stats;
        }

        private static int GetWeekNumber(DateTime date)
        {
            return System.Globalization.CultureInfo.InvariantCulture.Calendar
                .GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        private static string CalculateAverage(IEnumerable<double> hours)
        {
            var list = hours.ToList();
            if (!list.Any()) return "0h";
            var avg = list.Average();
            if (avg < 1) return $"{avg * 60:F0}m";
            if (avg < 24) return $"{avg:F1}h";
            return $"{avg / 24:F1}d";
        }
    }
}
