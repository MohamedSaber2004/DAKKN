using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.AssignTicket
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public AssignTicketCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<bool> Handle(AssignTicketCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await repo.GetByIdAsync(request.TicketId);

            var oldAssignee = ticket.AssignedToName;
            ticket.AssignTo(request.AdminId, request.AdminName);
            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserId.ToString(), "Assigned",
                $"Assigned to {request.AdminName}", oldAssignee, request.AdminName);
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(ticket.CustomerEmail))
            {
                try { await _emailService.SendTicketAssignedEmailAsync(ticket.CustomerEmail, ticket.CustomerName, ticket.TicketNumber, request.AdminName, ct); }
                catch { }
            }

            return true;
        }
    }
}
