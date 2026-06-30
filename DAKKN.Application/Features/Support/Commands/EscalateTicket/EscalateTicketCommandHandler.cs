using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.EscalateTicket
{
    public class EscalateTicketCommandHandler : IRequestHandler<EscalateTicketCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public EscalateTicketCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(EscalateTicketCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await repo.GetByIdAsync(request.TicketId);

            ticket.UpdatePriority(SupportTicketPriority.Urgent);
            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserName, "Escalated", request.Reason);
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
