using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.CreateShippingGovernorate
{
    public record CreateShippingGovernorateCommand(string Name, string ArName, decimal ShippingPrice, int DisplayOrder) : IRequest<ShippingGovernorateDto>;
}
