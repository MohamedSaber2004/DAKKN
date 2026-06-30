using MediatR;

namespace DAKKN.Application.Features.Support.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;
}
