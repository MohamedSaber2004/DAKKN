namespace DAKKN.Application.Features.Support.DTOs
{
    public class SupportTicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryArName { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public bool HasUnreadReplies { get; set; }
        public int ReplyCount { get; set; }
    }

    public class SupportTicketDetailsDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? AssignedToId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public string? OrderNumber { get; set; }
        public string? Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<SupportReplyDto> Replies { get; set; } = new();
        public List<SupportAttachmentDto> Attachments { get; set; } = new();
        public List<SupportActivityDto> Activities { get; set; } = new();
        public List<SupportInternalNoteDto> InternalNotes { get; set; } = new();
    }

    public class SupportReplyDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public bool IsStaffReply { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<SupportAttachmentDto> Attachments { get; set; } = new();
    }

    public class SupportAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public class SupportActivityDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SupportInternalNoteDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SupportCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class SupportFAQDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string ArQuestion { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string ArAnswer { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryArName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
    }

    public class SupportFAQCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ArName { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int FAQCount { get; set; }
    }

    public class SupportSettingsDto
    {
        public Guid Id { get; set; }
        public string SupportEmail { get; set; } = string.Empty;
        public string DefaultPriority { get; set; } = string.Empty;
        public string DefaultResponseTime { get; set; } = string.Empty;
        public int MaxAttachmentSize { get; set; }
        public string AllowedExtensions { get; set; } = string.Empty;
        public int AutoCloseDays { get; set; }
        public bool NotifyOnNewTicket { get; set; }
        public bool NotifyOnReply { get; set; }
        public bool NotifyOnAssignment { get; set; }
        public bool NotifyOnStatusChange { get; set; }
    }

    public class SupportDashboardRecentTicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class SupportDashboardStatsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int PendingTickets { get; set; }
        public int WaitingCustomer { get; set; }
        public int ClosedTickets { get; set; }
        public int HighPriorityTickets { get; set; }
        public string AverageResponseTime { get; set; } = "0h";
        public string AverageResolutionTime { get; set; } = "0h";
        public List<ChartDataPoint> TicketsByDay { get; set; } = new();
        public List<ChartDataPoint> TicketsByWeek { get; set; } = new();
        public List<ChartDataPoint> TicketsByMonth { get; set; } = new();
        public List<ChartDataPoint> PriorityDistribution { get; set; } = new();
        public List<ChartDataPoint> CategoryDistribution { get; set; } = new();
        public List<ChartDataPoint> StatusDistribution { get; set; } = new();
        public List<TopIssueDto> TopIssues { get; set; } = new();
        public List<SupportDashboardRecentTicketDto> RecentTickets { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class TopIssueDto
    {
        public string Subject { get; set; } = string.Empty;
        public int Count { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class CreateTicketResponseDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EstimatedResponseTime { get; set; } = string.Empty;
    }
}
