using DAKKN.Application.Common.Exceptions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Application.Localization;
using DAKKN.Domain.Entities;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings
{
    public class UpdateInventorySettingsCommandHandler : IRequestHandler<UpdateInventorySettingsCommand, InventorySettingsDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<Messages> _localizer;

        public UpdateInventorySettingsCommandHandler(IApplicationDbContext context, IStringLocalizer<Messages> localizer)
        {
            _context = context;
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

            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            if (setting == null)
            {
                setting = new SystemSetting
                {
                    Key = "GlobalDangerQuantity",
                    Value = request.GlobalDangerQuantity.ToString(),
                    Description = "Global default danger quantity threshold for low stock warning"
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = request.GlobalDangerQuantity.ToString();
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new InventorySettingsDto
            {
                GlobalDangerQuantity = request.GlobalDangerQuantity
            };
        }
    }
}
