using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string CategoryName) : IRequest<CategoryDto>;
}
