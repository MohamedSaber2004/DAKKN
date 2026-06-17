using DAKKN.Domain.Enums;

namespace DAKKN.Application.Features.Users.Queries.GetUserById
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public LanguageCode Language { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
