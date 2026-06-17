using DAKKN.Application.Common.Extensions;
using DAKKN.Application.Common.Interfaces;
using DAKKN.Application.Features.Users.Queries.GetAllUsers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAKKN.Application.Features.Users.Queries.ExportUsers
{
    public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, List<UserListItemDto>>
    {
        private readonly IApplicationDbContext _context;

        public ExportUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<UserListItemDto>> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsNoTracking();

            // Apply Filters (same as GetAllUsersQueryHandler)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(u => u.FullName.Contains(request.SearchTerm) || 
                                         u.Email.Contains(request.SearchTerm) || 
                                         u.PhoneNumber.Contains(request.SearchTerm));
            }

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

            var items = await query.Select(u => new UserListItemDto
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
            }).ToListAsync(cancellationToken);

            return items;
        }
    }
}
