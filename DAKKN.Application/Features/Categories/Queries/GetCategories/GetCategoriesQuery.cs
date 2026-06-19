using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery(string? SearchTerm = null, bool IncludeInactive = false) : IRequest<List<CategoryDto>>;
}
