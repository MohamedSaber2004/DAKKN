using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings
{
    public class UpdateInventorySettingsCommandHandler : IRequestHandler<UpdateInventorySettingsCommand, InventorySettingsDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateInventorySettingsCommandHandler(IUnitOfWork unitOfWork, IStringLocalizer<Messages> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<InventorySettingsDto> Handle(UpdateInventorySettingsCommand request, CancellationToken cancellationToken)
        {
            if (request.GlobalDangerQuantity < 0)
            {
                var failures = new List<ValidationFailure>
                {
                    new ValidationFailure(nameof(request.GlobalDangerQuantity),
                        _localizer[LocalizationKeys.ValidationMessages.GreaterThanOrEqual.Value, 0])
                };
                throw new ValidationException(failures);
            }

            var repo = _unitOfWork.GetRepository<SystemSetting>();
            var setting = await repo.GetFirstAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = "GlobalDangerQuantity",
                    Value = request.GlobalDangerQuantity.ToString(),
                    Description = "Global default danger quantity threshold for low stock warning"
                };
                await repo.AddAsync(setting);
            }
            else
            {
                setting.Value = request.GlobalDangerQuantity.ToString();
            }

            await _unitOfWork.SaveChangesAsync();

            return new InventorySettingsDto
            {
                GlobalDangerQuantity = request.GlobalDangerQuantity
            };
        }
    }
}
