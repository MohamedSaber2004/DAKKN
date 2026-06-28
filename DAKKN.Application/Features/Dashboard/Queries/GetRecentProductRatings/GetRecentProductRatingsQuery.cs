using MediatR;

namespace DAKKN.Application.Features.Dashboard.Queries.GetRecentProductRatings
{
    public record GetRecentProductRatingsQuery(int Count = 5) : IRequest<List<RecentProductRatingDto>>;
}
