using DAKKN.Domain.Common.Interfaces;
using DAKKN.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAKKN.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>, IBaseEntity<Guid>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public string FullName { get; private set; } = null!;
        public DateTime BirthDate { get; private set; }
        public Gender Gender { get; private set; }
        public string? ProfilePictureUrl { get; private set; }
        public string? GoogleUserId { get; private set; }
        public string? PasswordResetToken { get; private set; }
        public DateTime? PasswordResetTokenExpiry { get; private set; }
        public LanguageCode Language { get; private set; } = LanguageCode.en;

        private ApplicationUser() : base() { }

        public ApplicationUser(string userName, string email, string fullName, DateTime birthDate, Gender gender) : base(userName)
        {
            Id = Guid.NewGuid();
            Email = email;
            FullName = fullName;
            BirthDate = birthDate;
            Gender = gender;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }

        public static ApplicationUser Create(string fullName, string email, string phoneNumber)
        {
            var user = new ApplicationUser(email, email, fullName, new DateTime(1990, 1, 1), Gender.Male);
            user.PhoneNumber = phoneNumber;
            return user;
        }

        public void UpdateProfile(string fullName, DateTime birthDate, Gender gender, string? profilePictureUrl)
        {
            FullName = fullName;
            BirthDate = birthDate;
            Gender = gender;
            ProfilePictureUrl = profilePictureUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFullName(string fullName)
        {
            FullName = fullName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetGoogleUserId(string googleUserId)
        {
            GoogleUserId = googleUserId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPasswordResetToken(string token, DateTime expiry)
        {
            PasswordResetToken = token;
            PasswordResetTokenExpiry = expiry;
        }

        public void ClearPasswordResetToken()
        {
            PasswordResetToken = null;
            PasswordResetTokenExpiry = null;
        }

        public void UpdateLanguage(LanguageCode language)
        {
            Language = language;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete(string deletedBy)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; private set; } = new List<UserRefreshToken>();
    }
}
