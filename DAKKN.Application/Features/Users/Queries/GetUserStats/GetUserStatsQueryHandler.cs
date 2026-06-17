using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAKKN.Application.Features.Users.Queries.GetUserStats
{
    public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, GetUserStatsResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserStatsQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GetUserStatsResponse> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
        {
            var totalUsers = await _userManager.Users.AsNoTracking().CountAsync(cancellationToken);
            var activeUsers = await _userManager.Users.AsNoTracking().CountAsync(u => !u.IsDeleted && u.IsActive, cancellationToken);
            var deletedUsers = await _userManager.Users.AsNoTracking().CountAsync(u => u.IsDeleted || !u.IsActive, cancellationToken);

            return new GetUserStatsResponse(totalUsers, activeUsers, deletedUsers);
        }
    }
}
