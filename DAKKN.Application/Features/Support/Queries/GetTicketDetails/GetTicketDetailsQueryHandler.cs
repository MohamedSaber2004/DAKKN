using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetTicketDetails
{
    public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, SupportTicketDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public GetTicketDetailsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportTicketDetailsDto> Handle(GetTicketDetailsQuery request, CancellationToken ct)
        {
            var query = _unitOfWork.GetRepository<SupportTicket>()
                .GetAllAsync(t => t.Id == request.TicketId && !t.IsDeleted)
                .Include(t => t.Category)
                .Include(t => t.Replies).ThenInclude(r => r.Attachments)
                .Include(t => t.Attachments)
                .Include(t => t.Activities)
                .Include(t => t.InternalNotes)
                .AsQueryable();

            if (!request.IsAdmin)
                query = query.Where(t => t.CustomerId == _currentUserService.UserId);

            var ticket = await query.FirstOrDefaultAsync(ct);
            if (ticket == null)
                throw new NotFoundException("Ticket not found");

            return new SupportTicketDetailsDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Subject = ticket.Subject,
                Message = ticket.Message,
                CategoryName = ticket.Category.Name,
                CategoryArName = ticket.Category.ArName,
                CategoryId = ticket.CategoryId.ToString(),
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                CustomerName = ticket.CustomerName,
                CustomerEmail = ticket.CustomerEmail,
                CustomerPhone = ticket.CustomerPhone,
                AssignedToId = ticket.AssignedToId?.ToString(),
                AssignedToName = ticket.AssignedToName ?? "",
                OrderNumber = ticket.OrderNumber,
                Source = ticket.Source,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt,
                FirstResponseAt = ticket.FirstResponseAt,
                ResolvedAt = ticket.ResolvedAt,
                Replies = ticket.Replies.OrderBy(r => r.CreatedAt).Select(r => new SupportReplyDto
                {
                    Id = r.Id,
                    Message = r.Message,
                    UserName = r.UserName,
                    UserId = r.UserId,
                    IsStaffReply = r.IsStaffReply,
                    CreatedAt = r.CreatedAt,
                    Attachments = r.Attachments.Select(a => new SupportAttachmentDto
                    {
                        Id = a.Id,
                        FileName = a.FileName,
                        OriginalFileName = a.OriginalFileName,
                        ContentType = a.ContentType,
                        FileSize = a.FileSize,
                        FilePath = a.FilePath,
                        Url = $"/files/{System.IO.Path.GetFileName(a.FilePath)}"
                    }).ToList()
                }).ToList(),
                Attachments = ticket.Attachments.Where(a => a.ReplyId == null).Select(a => new SupportAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    FilePath = a.FilePath,
                    Url = $"/files/{System.IO.Path.GetFileName(a.FilePath)}"
                }).ToList(),
                Activities = ticket.Activities.OrderByDescending(a => a.CreatedAt).Select(a => new SupportActivityDto
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Action = a.Action,
                    Details = a.Details,
                    OldValue = a.OldValue,
                    NewValue = a.NewValue,
                    CreatedAt = a.CreatedAt
                }).ToList(),
                InternalNotes = request.IsAdmin
                    ? ticket.InternalNotes.OrderByDescending(n => n.CreatedAt).Select(n => new SupportInternalNoteDto
                    {
                        UserName = n.UserName,
                        Note = n.Note,
                        CreatedAt = n.CreatedAt
                    }).ToList()
                    : new List<SupportInternalNoteDto>()
            };
        }
    }
}
