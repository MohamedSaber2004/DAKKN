using System;

namespace DAKKN.MVC.ViewModels.Admin
{
    public class SupportTicketViewModel
    {
        public string TicketId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string MessageSnippet { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
    }

    public enum TicketStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public class SupportDashboardViewModel
    {
        public List<SupportTicketViewModel> Tickets { get; set; } = new();
        public int TotalOpen { get; set; }
        public int WaitingResponse { get; set; }
        public string AverageResolutionTime { get; set; } = "24h";
    }
}
