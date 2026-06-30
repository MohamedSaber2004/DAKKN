using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Support.Commands.UpdateSettings
{
    public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, SupportSettingsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateSettingsCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<SupportSettingsDto> Handle(UpdateSettingsCommand request, CancellationToken ct)
        {
            var repo = _unitOfWork.GetRepository<SupportSettings>();
            var settings = repo.GetAllAsync(_ => true).FirstOrDefault();
            if (settings == null)
            {
                settings = SupportSettings.CreateDefault();
                await repo.AddAsync(settings);
            }

            settings.SupportEmail = request.SupportEmail;
            settings.DefaultPriority = request.DefaultPriority;
            settings.DefaultResponseTime = request.DefaultResponseTime;
            settings.MaxAttachmentSize = request.MaxAttachmentSize;
            settings.AllowedExtensions = request.AllowedExtensions;
            settings.AutoCloseDays = request.AutoCloseDays;
            settings.NotifyOnNewTicket = request.NotifyOnNewTicket;
            settings.NotifyOnReply = request.NotifyOnReply;
            settings.NotifyOnAssignment = request.NotifyOnAssignment;
            settings.NotifyOnStatusChange = request.NotifyOnStatusChange;
            settings.MarkAsUpdated(_currentUserService.UserId.ToString());

            repo.Update(settings);
            await _unitOfWork.SaveChangesAsync();

            return new SupportSettingsDto
            {
                Id = settings.Id,
                SupportEmail = settings.SupportEmail,
                DefaultPriority = settings.DefaultPriority,
                DefaultResponseTime = settings.DefaultResponseTime,
                MaxAttachmentSize = settings.MaxAttachmentSize,
                AllowedExtensions = settings.AllowedExtensions,
                AutoCloseDays = settings.AutoCloseDays,
                NotifyOnNewTicket = settings.NotifyOnNewTicket,
                NotifyOnReply = settings.NotifyOnReply,
                NotifyOnAssignment = settings.NotifyOnAssignment,
                NotifyOnStatusChange = settings.NotifyOnStatusChange
            };
        }
    }
}
