using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.DeleteShippingGovernorate
{
    public record DeleteShippingGovernorateCommand(Guid Id) : IRequest;
}
