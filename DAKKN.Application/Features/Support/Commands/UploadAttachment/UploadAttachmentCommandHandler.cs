using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UploadAttachment
{
    public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, SupportAttachmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageValidator _imageValidator;

        public UploadAttachmentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IImageValidator imageValidator)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _imageValidator = imageValidator;
        }

        public async Task<SupportAttachmentDto> Handle(UploadAttachmentCommand request, CancellationToken ct)
        {
            var file = request.Files.First();
            var (uploaded, result) = await _imageValidator.UploadImage(file, 7);
            if (!uploaded || result == null)
                throw new Exception("Upload failed");

            var attachment = SupportAttachment.Create(
                request.TicketId, result, file.FileName,
                file.ContentType, file.Length, result, request.ReplyId);

            var repo = _unitOfWork.GetRepository<SupportAttachment>();
            await repo.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();

            return new SupportAttachmentDto
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                OriginalFileName = attachment.OriginalFileName,
                ContentType = attachment.ContentType,
                FileSize = attachment.FileSize,
                FilePath = attachment.FilePath
            };
        }
    }
}
