using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetCategories
{
    public record GetCategoriesQuery(bool IncludeInactive = false) : IRequest<List<SupportCategoryDto>>;
}
