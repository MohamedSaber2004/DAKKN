using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateTicketPriority
{
    public class UpdateTicketPriorityCommandHandler : IRequestHandler<UpdateTicketPriorityCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateTicketPriorityCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(UpdateTicketPriorityCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await repo.GetByIdAsync(request.TicketId);

            if (!Enum.TryParse<SupportTicketPriority>(request.NewPriority, true, out var newPriority))
                return false;

            var oldPriority = ticket.Priority.ToString();
            ticket.UpdatePriority(newPriority);
            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserId.ToString(), "PriorityChanged",
                "Priority changed", oldPriority, newPriority.ToString());
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
