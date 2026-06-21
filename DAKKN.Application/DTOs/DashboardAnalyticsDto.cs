namespace DAKKN.Application.DTOs
{
    public class DashboardAnalyticsDto
    {
        public List<DailyAnalytics> DailyData { get; set; } = new();
    }

    public class DailyAnalytics
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrdersCount { get; set; }
        public DateOnly Date { get; set; }
    }
}
