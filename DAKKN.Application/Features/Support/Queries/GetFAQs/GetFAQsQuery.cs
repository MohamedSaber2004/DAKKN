using DAKKN.Application.Features.Support.DTOs;
using MediatR;

namespace DAKKN.Application.Features.Support.Queries.GetFAQs
{
    public record GetFAQsQuery(Guid? CategoryId = null, bool OnlyPublished = true) : IRequest<List<SupportFAQDto>>;
}
