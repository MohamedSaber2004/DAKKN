using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateSettings
{
    public record UpdateSettingsCommand : IRequest<SupportSettingsDto>
    {
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
}
