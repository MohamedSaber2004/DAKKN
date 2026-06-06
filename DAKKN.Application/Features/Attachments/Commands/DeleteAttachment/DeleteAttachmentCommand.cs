using DAKKN.Application.Common.Models;
using MediatR;

namespace DAKKN.Application.Features.Attachments.Commands.DeleteAttachment
{
    public class DeleteAttachmentCommand : IRequest<FileDeleteResult>
    {
        public Guid Id { get; set; }
    }
}
