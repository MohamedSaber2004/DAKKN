using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Inventory.Commands.UpdateInventorySettings
{
    public record UpdateInventorySettingsCommand(int GlobalDangerQuantity) : IRequest<InventorySettingsDto>;
}
