using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteAttachment
{
    public class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;

        public DeleteAttachmentCommandHandler(IUnitOfWork unitOfWork, IImageValidator imageValidator)
        {
            _unitOfWork = unitOfWork;
            _imageValidator = imageValidator;
        }

        public async Task<bool> Handle(DeleteAttachmentCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportAttachment>();
            var attachment = await repo.GetByIdAsync(request.AttachmentId);

            await _imageValidator.DeleteImage(attachment.FilePath, 7);
            repo.Delete(attachment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
