using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Inventory.Queries.GetInventorySettings
{
    public record GetInventorySettingsQuery : IRequest<InventorySettingsDto>;
}
