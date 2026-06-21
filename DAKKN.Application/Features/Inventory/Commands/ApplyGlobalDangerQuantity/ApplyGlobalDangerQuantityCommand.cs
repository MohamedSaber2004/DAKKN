using MediatR;

namespace DAKKN.Application.Features.Inventory.Commands.ApplyGlobalDangerQuantity
{
    public record ApplyGlobalDangerQuantityCommand : IRequest<int>;
}
