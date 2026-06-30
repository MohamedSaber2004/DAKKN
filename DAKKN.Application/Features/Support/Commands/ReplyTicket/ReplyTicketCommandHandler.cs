using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.ReplyTicket
{
    public class ReplyTicketCommandHandler : IRequestHandler<ReplyTicketCommand, SupportReplyDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageValidator _imageValidator;
        private readonly IEmailService _emailService;

        public ReplyTicketCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IImageValidator imageValidator, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _imageValidator = imageValidator;
            _emailService = emailService;
        }

        public async Task<SupportReplyDto> Handle(ReplyTicketCommand request, CancellationToken ct)
        {
            var ticketRepo = _unitOfWork.GetRepository<SupportTicket>();
            var ticket = await ticketRepo.GetByIdAsync(request.TicketId);

            var reply = SupportReply.Create(
                request.TicketId,
                _currentUserService.UserId,
                _currentUserService.UserId.ToString(),
                request.Message,
                request.IsStaffReply);

            var replyRepo = _unitOfWork.GetRepository<SupportReply>();
            await replyRepo.AddAsync(reply);

            if (request.Attachments?.Any() == true)
            {
                foreach (var file in request.Attachments)
                {
                    var (uploaded, result) = await _imageValidator.UploadImage(file, 7);
                    if (uploaded && result != null)
                    {
                        var attachment = SupportAttachment.Create(
                            request.TicketId, result, file.FileName,
                            file.ContentType, file.Length, result, reply.Id);
                        var attachRepo = _unitOfWork.GetRepository<SupportAttachment>();
                        await attachRepo.AddAsync(attachment);
                    }
                }
            }

            if (request.IsStaffReply)
                ticket.RecordFirstResponse();

            ticket.MarkAsUpdated(_currentUserService.UserId.ToString());

            var status = request.IsStaffReply ? SupportTicketStatus.WaitingCustomer : SupportTicketStatus.WaitingStaff;
            ticket.UpdateStatus(status);

            var activity = SupportActivity.Create(request.TicketId, _currentUserService.UserId,
                _currentUserService.UserId.ToString(), "Replied", "New reply added");
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();

            if (request.IsStaffReply && !string.IsNullOrEmpty(ticket.CustomerEmail))
            {
                _ = SafeSendEmailAsync(() =>
                    _emailService.SendTicketReplyEmailAsync(ticket.CustomerEmail, ticket.CustomerName,
                        ticket.TicketNumber, request.Message, true, ct));
            }

            return new SupportReplyDto
            {
                Id = reply.Id,
                Message = reply.Message,
                UserName = reply.UserName,
                UserId = reply.UserId,
                IsStaffReply = reply.IsStaffReply,
                CreatedAt = reply.CreatedAt
            };
        }

        private static async Task SafeSendEmailAsync(Func<Task> sendAction)
        {
            try { await sendAction(); }
            catch { }
        }
    }
}
