using DAKKN.Application.Common.Models;
using MediatR;

namespace DAKKN.Application.Features.Attachments.Commands.UploadAttachment
{
    public class UploadAttachmentCommand : IRequest<UploadResult>
    {
        public Stream Stream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int Place { get; set; }
        public string ContentType { get; set; } = null!;
    }
}
