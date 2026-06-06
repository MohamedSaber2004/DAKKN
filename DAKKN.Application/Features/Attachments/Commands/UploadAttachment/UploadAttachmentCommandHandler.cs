using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Attachments.Commands.UploadAttachment
{
    public class UploadAttachmentCommandHandler(IFileStorageService storageService, IUnitOfWork ctx) 
        : IRequestHandler<UploadAttachmentCommand, UploadResult>
    {
        public async Task<UploadResult> Handle(UploadAttachmentCommand request, CancellationToken ct)
        {
            var result = await storageService.UploadAsync(request.Stream, request.FileName, request.Place, request.ContentType, ct);

            if (result.Succeeded)
            {
                var attachment = new Attachment(
                    result.FileName!, 
                    result.FileId, 
                    request.Place, 
                    request.Stream.Length, 
                    request.ContentType);

                await ctx.GetRepository<Attachment>().AddAsync(attachment);
                await ctx.SaveChangesAsync();
            }

            return result;
        }
    }
}
