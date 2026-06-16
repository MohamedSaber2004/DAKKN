using System.Collections.Generic;

namespace DAKKN.Application.Features.Users.Queries.GetAllUsers
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime JoinDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
