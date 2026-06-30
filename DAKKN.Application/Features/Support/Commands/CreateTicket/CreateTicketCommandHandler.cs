using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Support.Commands.CreateTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketResponseDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly IImageValidator _imageValidator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public CreateTicketCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer, IImageValidator imageValidator,
            UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
            _imageValidator = imageValidator;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<CreateTicketResponseDto> Handle(CreateTicketCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
            var customerName = user?.FullName ?? _currentUserService.UserId.ToString();
            var customerEmail = user?.Email ?? string.Empty;

            var ticket = SupportTicket.Create(
                _currentUserService.UserId,
                customerName,
                customerEmail,
                request.Subject,
                request.Message,
                request.CategoryId,
                request.PhoneNumber,
                request.Source,
                request.OrderNumber);

            var repo = _unitOfWork.GetRepository<SupportTicket>();
            await repo.AddAsync(ticket);

            if (request.Attachments?.Any() == true)
            {
                foreach (var file in request.Attachments)
                {
                    var (uploaded, result) = await _imageValidator.UploadImage(file, 7);
                    if (uploaded && result != null)
                    {
                        var attachment = SupportAttachment.Create(
                            ticket.Id, result, file.FileName,
                            file.ContentType, file.Length, result);
                        var attachRepo = _unitOfWork.GetRepository<SupportAttachment>();
                        await attachRepo.AddAsync(attachment);
                    }
                }
            }

            var activity = SupportActivity.Create(ticket.Id, _currentUserService.UserId,
                _currentUserService.UserId.ToString(), "Created", "Ticket created");
            var activityRepo = _unitOfWork.GetRepository<SupportActivity>();
            await activityRepo.AddAsync(activity);

            await _unitOfWork.SaveChangesAsync();

            _ = SafeSendEmailAsync(() =>
                _emailService.SendTicketCreatedEmailAsync(customerEmail, customerName, ticket.TicketNumber, ticket.Subject, ct));

            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins.Where(a => !string.IsNullOrEmpty(a.Email)))
            {
                var adminName = string.IsNullOrEmpty(admin.FullName) ? admin.Email! : admin.FullName;
                _ = SafeSendEmailAsync(() =>
                    _emailService.SendNewTicketNotificationToAdminAsync(admin.Email!, adminName, ticket.TicketNumber, customerName, ticket.Subject, ct));
            }

            return new CreateTicketResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Status = ticket.Status.ToString(),
                EstimatedResponseTime = _localizer[LocalizationKeys.Support.DefaultResponseTime.Value]
            };
        }
        private static async Task SafeSendEmailAsync(Func<Task> sendAction)
        {
            try { await sendAction(); }
            catch { }
        }
    }
}
