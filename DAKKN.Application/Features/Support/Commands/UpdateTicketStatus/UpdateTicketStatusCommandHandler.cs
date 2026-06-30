using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateTicketStatus
{
    public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTicketStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateTicketStatusCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await repo.GetByIdAsync(request.TicketId);

            if (!Enum.TryParse<SupportTicketStatus>(request.NewStatus, true, out var newStatus))
                return false;

            var oldStatus = ticket.Status.ToString();
            ticket.UpdateStatus(newStatus);
            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserId.ToString(), "StatusChanged",
                "Status changed", oldStatus, newStatus.ToString());
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
