using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.UpdateShippingGovernorate
{
    public record UpdateShippingGovernorateCommand(Guid Id, string Name, string ArName, decimal ShippingPrice, int DisplayOrder, bool IsActive) : IRequest<ShippingGovernorateDto>;
}
