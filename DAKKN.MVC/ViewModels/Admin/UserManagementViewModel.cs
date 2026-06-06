using System;
using System.Collections.Generic;

namespace DAKKN.MVC.ViewModels.Admin
{
    public enum UserRole
    {
        Admin,
        Designer,
        Customer
    }

    public enum UserStatus
    {
        Active,
        Blocked,
        UnderReview
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class UserManagementViewModel
    {
        public List<UserListItemViewModel> Users { get; set; } = new();
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int DesignersCount { get; set; }
        public int BlockedUsers { get; set; }
    }
}
