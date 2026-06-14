using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAKKN.Application.Features.Auth.Comands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public LogoutCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<string> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var tokenEntity = await _unitOfWork.GetRepository<UserRefreshToken>()
                .GetFirstAsync(x => x.Token == request.RefreshToken, cancellationToken);

            if (tokenEntity != null)
            {
                tokenEntity.Revoke();
                _unitOfWork.GetRepository<UserRefreshToken>().Update(tokenEntity);
                await _unitOfWork.SaveChangesAsync();
            }

            return JsonLocalizationProvider.GetLocalizedString(_localizer[LocalizationKeys.AuthMessages.LogoutSuccess.Value]);
        }
    }
}
