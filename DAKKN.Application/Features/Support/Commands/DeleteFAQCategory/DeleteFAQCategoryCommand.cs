using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteFAQCategory
{
    public record DeleteFAQCategoryCommand(Guid Id) : IRequest<bool>;
}
