using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.AddInternalNote
{
    public class AddInternalNoteCommandHandler : IRequestHandler<AddInternalNoteCommand, SupportInternalNoteDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddInternalNoteCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportInternalNoteDto> Handle(AddInternalNoteCommand request, CancellationToken ct)
        {
            var note = SupportInternalNote.Create(
                request.TicketId,
                _currentUserService.UserId,
                _currentUserService.UserId.ToString(),
                request.Note);

            var repo = _unitOfWork.GetRepository<SupportInternalNote>();
            await repo.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();

            return new SupportInternalNoteDto
            {
                Id = note.Id,
                UserName = note.UserName,
                Note = note.Note,
                CreatedAt = note.CreatedAt
            };
        }
    }
}
