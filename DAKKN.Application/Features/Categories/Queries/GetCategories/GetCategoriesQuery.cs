using DAKKN.Application.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Categories.Queries.GetCategories
{
    public record GetCategoriesQuery(string? SearchTerm = null, bool IncludeInactive = false, int? Top = null) : IRequest<List<CategoryDto>>;
}
