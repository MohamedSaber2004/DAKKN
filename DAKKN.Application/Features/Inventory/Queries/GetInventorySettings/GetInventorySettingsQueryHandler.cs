using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using DAKKN.Domain.Repositories.Interfaces.Base;
using MediatR;

namespace DAKKN.Application.Features.Inventory.Queries.GetInventorySettings
{
    public class GetInventorySettingsQueryHandler : IRequestHandler<GetInventorySettingsQuery, InventorySettingsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInventorySettingsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InventorySettingsDto> Handle(GetInventorySettingsQuery request, CancellationToken cancellationToken)
        {
            var setting = await _unitOfWork.GetRepository<SystemSetting>()
                .GetFirstAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            return new InventorySettingsDto
            {
                GlobalDangerQuantity = setting != null && int.TryParse(setting.Value, out var val) ? val : 10
            };
        }
    }
}
