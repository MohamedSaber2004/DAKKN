using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.DTOs;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Inventory.Queries.GetInventorySettings
{
    public class GetInventorySettingsQueryHandler : IRequestHandler<GetInventorySettingsQuery, InventorySettingsDto>
    {
        private readonly IApplicationDbContext _context;

        public GetInventorySettingsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventorySettingsDto> Handle(GetInventorySettingsQuery request, CancellationToken cancellationToken)
        {
            var setting = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "GlobalDangerQuantity", cancellationToken);

            return new InventorySettingsDto
            {
                GlobalDangerQuantity = setting != null && int.TryParse(setting.Value, out var val) ? val : 10
            };
        }
    }
}
