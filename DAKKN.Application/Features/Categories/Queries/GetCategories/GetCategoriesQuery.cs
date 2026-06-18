using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery(string? SearchTerm = null) : IRequest<List<CategoryDto>>;
}
