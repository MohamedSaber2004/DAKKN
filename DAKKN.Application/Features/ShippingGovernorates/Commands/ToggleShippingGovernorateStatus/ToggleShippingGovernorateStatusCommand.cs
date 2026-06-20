using MediatR;

namespace DAKKN.Application.Features.ShippingGovernorates.Commands.ToggleShippingGovernorateStatus
{
    public record ToggleShippingGovernorateStatusCommand(Guid Id) : IRequest;
}
