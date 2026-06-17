using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAKKN.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagginatedResult<UserListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagginatedResult<UserListItemDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking();

            // Apply Filters
            query = query.WhereIf(!string.IsNullOrWhiteSpace(request.FullName), u => u.FullName.Contains(request.FullName!))
                         .WhereIf(!string.IsNullOrWhiteSpace(request.Email), u => u.Email.Contains(request.Email!))
                         .WhereIf(!string.IsNullOrWhiteSpace(request.PhoneNumber), u => u.PhoneNumber.Contains(request.PhoneNumber!));

            // Status logic: Inactive is Deleted in this business
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (request.Status == "Deleted")
                    query = query.Where(u => u.IsDeleted || !u.IsActive);
                else if (request.Status == "Active")
                    query = query.Where(u => !u.IsDeleted && u.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var roleId = await _context.Roles
                    .Where(r => r.Name == request.Role)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (roleId != Guid.Empty)
                {
                    query = query.Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
                }
            }

            query = query.OrderByDescending(u => u.CreatedAt);

            var paginatedUsers = await query
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    JoinDate = u.CreatedAt,
                    Status = (u.IsDeleted || !u.IsActive) ? "Deleted" : "Active",
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name!)
                        .ToList()
                })
                .AsPagginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            return paginatedUsers;
        }
    }
}
