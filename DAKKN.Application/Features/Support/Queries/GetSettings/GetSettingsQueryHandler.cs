using DAKKN.Application.Features.Support.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Support.Queries.GetSettings
{
    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, SupportSettingsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSettingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportSettingsDto> Handle(GetSettingsQuery request, CancellationToken ct)
        {
            var settings = await _unitOfWork.GetRepository<SupportSettings>().GetAllAsync(_ => true).FirstOrDefaultAsync(ct);
            if (settings == null)
            {
                settings = SupportSettings.CreateDefault();
                var repo = _unitOfWork.GetRepository<SupportSettings>();
                await repo.AddAsync(settings);
                await _unitOfWork.SaveChangesAsync();
            }

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
