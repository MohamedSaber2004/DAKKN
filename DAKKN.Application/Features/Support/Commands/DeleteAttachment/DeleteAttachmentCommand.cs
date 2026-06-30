using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteAttachment
{
    public record DeleteAttachmentCommand(Guid AttachmentId) : IRequest<bool>;
}
