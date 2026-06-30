using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteFAQ
{
    public record DeleteFAQCommand(Guid Id) : IRequest<bool>;
}
