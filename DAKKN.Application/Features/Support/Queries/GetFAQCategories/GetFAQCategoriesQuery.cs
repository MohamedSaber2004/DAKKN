using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetFAQCategories
{
    public record GetFAQCategoriesQuery : IRequest<List<SupportCategoryDto>>;
}
