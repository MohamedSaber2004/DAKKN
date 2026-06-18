using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
}
