using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.CloseTicket
{
    public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;

        public CloseTicketCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _emailService = emailService;
        }

        public async Task<bool> Handle(CloseTicketCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await repo.GetByIdAsync(request.TicketId);

            ticket.UpdateStatus(SupportTicketStatus.Closed);
            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserName, "Closed", "Ticket closed");
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(ticket.CustomerEmail))
            {
                try { await _emailService.SendTicketClosedEmailAsync(ticket.CustomerEmail, ticket.CustomerName, ticket.TicketNumber, ct); }
                catch { }
            }

            return true;
        }
    }
}
