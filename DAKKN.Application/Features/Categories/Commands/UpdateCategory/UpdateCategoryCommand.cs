using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string CategoryName, string ArName, bool IsActive = true) : IRequest<CategoryDto>;
}
