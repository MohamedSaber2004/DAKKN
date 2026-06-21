using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Dashboard.Queries.GetDashboardAnalytics
{
    public class GetDashboardAnalyticsQueryHandler : IRequestHandler<GetDashboardAnalyticsQuery, DashboardAnalyticsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetDashboardAnalyticsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardAnalyticsDto> Handle(GetDashboardAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-(request.Days - 1));

            var rawData = await _context.Orders
                .Where(o => !o.IsDeleted && o.CreatedAt >= startDate)
                .Select(o => new
                {
                    o.CreatedAt,
                    o.TotalAmount,
                    o.Status
                })
                .ToListAsync(cancellationToken);

            var grouped = rawData
                .GroupBy(o => DateOnly.FromDateTime(o.CreatedAt))
                .Select(g => new DailyAnalytics
                {
                    Date = g.Key,
                    OrdersCount = g.Count(),
                    Revenue = g.Where(x => x.Status == OrderStatus.Delivered).Sum(x => x.TotalAmount),
                    Label = string.Empty
                })
                .ToDictionary(d => d.Date);

            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var isArabic = culture.Name.StartsWith("ar");
            var result = new List<DailyAnalytics>();

            for (int i = 0; i < request.Days; i++)
            {
                var date = DateOnly.FromDateTime(startDate.AddDays(i));

                if (grouped.TryGetValue(date, out var existing))
                {
                    existing.Label = GetDayLabel(date, isArabic);
                    result.Add(existing);
                }
                else
                {
                    result.Add(new DailyAnalytics
                    {
                        Date = date,
                        Label = GetDayLabel(date, isArabic),
                        OrdersCount = 0,
                        Revenue = 0
                    });
                }
            }

            return new DashboardAnalyticsDto { DailyData = result };
        }

        private static string GetDayLabel(DateOnly date, bool isArabic)
        {
            var dayOfWeek = date.DayOfWeek;
            var shortNames = isArabic
                ? new[] { "أحد", "اثنين", "ثلاثاء", "أربعاء", "خميس", "جمعة", "سبت" }
                : new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            return shortNames[(int)dayOfWeek];
        }
    }
}
