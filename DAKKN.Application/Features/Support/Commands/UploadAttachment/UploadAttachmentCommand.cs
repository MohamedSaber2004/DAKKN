using DAKKN.Application.Features.Support.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DAKKN.Application.Features.Support.Commands.UploadAttachment
{
    public record UploadAttachmentCommand : IRequest<SupportAttachmentDto>
    {
        public Guid TicketId { get; set; }
        public Guid? ReplyId { get; set; }
        public List<IFormFile> Files { get; set; } = new();
    }
}
