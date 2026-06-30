using DAKKN.Domain.Enums;

namespace DAKKN.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
        string? IpAddress { get; }
        UserType UserType { get; }
    }
}
