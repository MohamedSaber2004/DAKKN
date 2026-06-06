using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces;
using DAKKN.Domain.Repositories.Interfaces.Base;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Attachments.Commands.DeleteAttachment
{
    public class DeleteAttachmentCommandValidator: AbstractValidator<DeleteAttachmentCommand>
    {
        private readonly IUnitOfWork _ctx;
        private readonly IStringLocalizer<Messages> _localizer;

        public DeleteAttachmentCommandValidator(IStringLocalizer<Messages> localizer, IUnitOfWork ctx)
        {
            _localizer = localizer;
            _ctx = ctx;

            RuleFor(x => x.Id)
                .NotEmpty()
                .MustAsync(AttachmentIdFound)
                .WithMessage(_localizer[LocalizationKeys.Attachment.AttachmentNotFound.Key]);
        }

        private async Task<bool> AttachmentIdFound(Guid attachmentId, CancellationToken cancellationToken)
        {
            return await _ctx.GetRepository<Attachment>().ExistsAsync(x => x.Id == attachmentId, cancellationToken);
        }
    }
}
