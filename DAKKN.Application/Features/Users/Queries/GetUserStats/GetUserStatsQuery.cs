using MediatR;

namespace DAKKN.Application.Features.Users.Queries.GetUserStats
{
    public record GetUserStatsResponse(int TotalUsers, int ActiveUsers, int DeletedUsers);

    public record GetUserStatsQuery : IRequest<GetUserStatsResponse>;
}
