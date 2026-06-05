using System;
using System.Collections.Generic;
using System.Linq;

namespace DAKKN.MVC.Mock
{
    /// <summary>
    /// Temporary in-memory store for UI prototyping.
    /// Replace with real Application layer commands when ready.
    /// </summary>
    public static class AuthMockStore
    {
        // Seed one known user so login works out of the box
        public static readonly List<MockUser> Users = new()
        {
            new MockUser
            {
                Id          = Guid.NewGuid(),
                FullName    = "Ahmed Hassan",
                Email       = "ahmed@dakkn.com",
                Password    = "Password123",   // plain text — mock only
                IsVerified  = true
            }
        };

        // Last generated OTP per email (plain, no hashing in mock)
        public static readonly Dictionary<string, string> PendingOtps = new();

        // Emails that have passed OTP and are cleared to reset password
        public static readonly HashSet<string> ResetApproved = new();

        public static MockUser? FindByEmail(string email) =>
            Users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        public static string IssueOtp(string email)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();
            PendingOtps[email.ToLower()] = code;
            return code;   // In real flow this gets emailed; here just stored
        }

        public static bool ValidateOtp(string email, string code)
        {
            var key = email.ToLower();
            return PendingOtps.TryGetValue(key, out var stored) && stored == code;
        }
    }

    public class MockUser
    {
        public Guid   Id        { get; set; }
        public string FullName  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;
        public string Password  { get; set; } = string.Empty;
        public bool   IsVerified { get; set; }
    }
}
