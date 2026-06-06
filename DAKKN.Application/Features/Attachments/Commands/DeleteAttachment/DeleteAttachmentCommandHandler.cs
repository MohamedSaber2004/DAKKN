using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Attachments.Commands.DeleteAttachment
{
    public class DeleteAttachmentCommandHandler(
        IFileStorageService storageService, 
        IUnitOfWork ctx) 
        : IRequestHandler<DeleteAttachmentCommand, FileDeleteResult>
    {
        public async Task<FileDeleteResult> Handle(DeleteAttachmentCommand request, CancellationToken ct)
        {
            var attachment = await ctx.GetRepository<Attachment>().GetFirstAsync(x => x.Id == request.Id, ct);

            var result = await storageService.DeleteAsync(attachment?.B2FileId ?? string.Empty, attachment!.FileName, ct);

            if (result.Succeeded)
            {
                ctx.GetRepository<Attachment>().Delete(attachment);
                await ctx.SaveChangesAsync();
            }

            return result;
        }
    }
}
