using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Models;
using DAKKN.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAKKN.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagginatedResult<UserListItemDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagginatedResult<UserListItemDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _userManager.Users.AsNoTracking();

            // Apply Filters
            query = query.WhereIf(!string.IsNullOrWhiteSpace(request.FullName), u => u.FullName.Contains(request.FullName!))
                         .WhereIf(!string.IsNullOrWhiteSpace(request.Email), u => u.Email.Contains(request.Email!))
                         .WhereIf(!string.IsNullOrWhiteSpace(request.PhoneNumber), u => u.PhoneNumber.Contains(request.PhoneNumber!))
                         .WhereIf(!string.IsNullOrWhiteSpace(request.Status), u => u.IsDeleted == (request.Status == "Deleted"));

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(request.Role);
                var userIds = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => userIds.Contains(u.Id));
            }

            var paginatedUsers = await query.AsPagginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            var items = new List<UserListItemDto>();
            foreach (var u in paginatedUsers.Items)
            {
                var roles = await _userManager.GetRolesAsync(u);
                items.Add(new UserListItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Roles = roles,
                    JoinDate = u.CreatedAt,
                    Status = u.IsDeleted ? "Deleted" : "Active"
                });
            }

            return PagginatedResult<UserListItemDto>.Create(items, paginatedUsers.TotalCount, request.PageNumber, request.PageSize);
        }
    }
}
