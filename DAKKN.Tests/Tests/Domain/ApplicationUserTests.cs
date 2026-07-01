using DAKKN.Domain.Entities;
using DAKKN.Domain.Enums;

namespace DAKKN.Tests.Tests.Domain;

public class ApplicationUserTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var birthDate = new DateTime(1995, 5, 15);
        var user = new ApplicationUser("john_doe", "john@test.com", "John Doe", birthDate, Gender.Male);

        user.Id.Should().NotBeEmpty();
        user.UserName.Should().Be("john_doe");
        user.Email.Should().Be("john@test.com");
        user.FullName.Should().Be("John Doe");
        user.BirthDate.Should().Be(birthDate);
        user.Gender.Should().Be(Gender.Male);
        user.IsActive.Should().BeTrue();
        user.IsDeleted.Should().BeFalse();
        user.Language.Should().Be(LanguageCode.en);
    }

    [Fact]
    public void Create_ShouldReturnUser_WithDefaults()
    {
        var user = ApplicationUser.Create("Jane Doe", "jane@test.com", "987654321");

        user.FullName.Should().Be("Jane Doe");
        user.Email.Should().Be("jane@test.com");
        user.PhoneNumber.Should().Be("987654321");
        user.UserName.Should().Be("jane@test.com");
        user.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        user.Gender.Should().Be(Gender.Male);
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateProfile_ShouldModifyProfileFields()
    {
        var user = new ApplicationUser("u", "e@e.com", "Old", DateTime.UtcNow, Gender.Male);
        var newBirthDate = new DateTime(2000, 1, 1);

        user.UpdateProfile("New Name", newBirthDate, Gender.Female, "http://img.com/pic.jpg");

        user.FullName.Should().Be("New Name");
        user.BirthDate.Should().Be(newBirthDate);
        user.Gender.Should().Be(Gender.Female);
        user.ProfilePictureUrl.Should().Be("http://img.com/pic.jpg");
    }

    [Fact]
    public void UpdateProfile_ShouldSetUpdatedAt()
    {
        var user = new ApplicationUser("u", "e@e.com", "Old", DateTime.UtcNow, Gender.Male);
        user.UpdateProfile("New", DateTime.UtcNow, Gender.Male, null);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateFullName_ShouldChangeName()
    {
        var user = new ApplicationUser("u", "e@e.com", "Old Name", DateTime.UtcNow, Gender.Male);
        user.UpdateFullName("New Name");

        user.FullName.Should().Be("New Name");
    }

    [Fact]
    public void UpdateFullName_ShouldSetUpdatedAt()
    {
        var user = new ApplicationUser("u", "e@e.com", "Old", DateTime.UtcNow, Gender.Male);
        user.UpdateFullName("New");
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetGoogleUserId_ShouldSetGoogleId()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.SetGoogleUserId("google-123");

        user.GoogleUserId.Should().Be("google-123");
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetPasswordResetToken_ShouldSetTokenAndExpiry()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        var expiry = DateTime.UtcNow.AddHours(1);

        user.SetPasswordResetToken("reset-token", expiry);

        user.PasswordResetToken.Should().Be("reset-token");
        user.PasswordResetTokenExpiry.Should().Be(expiry);
    }

    [Fact]
    public void ClearPasswordResetToken_ShouldClearTokenAndExpiry()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.SetPasswordResetToken("token", DateTime.UtcNow.AddHours(1));
        user.ClearPasswordResetToken();

        user.PasswordResetToken.Should().BeNull();
        user.PasswordResetTokenExpiry.Should().BeNull();
    }

    [Fact]
    public void UpdateLanguage_ShouldChangeLanguage()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.Language.Should().Be(LanguageCode.en);

        user.UpdateLanguage(LanguageCode.ar);

        user.Language.Should().Be(LanguageCode.ar);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.SoftDelete("admin-123");

        user.IsDeleted.Should().BeTrue();
        user.DeletedBy.Should().Be("admin-123");
        user.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Restore_ShouldClearDeletedFlags()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.SoftDelete("admin");
        user.Restore();

        user.IsDeleted.Should().BeFalse();
        user.DeletedBy.Should().BeNull();
        user.DeletedAt.Should().BeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activate_ShouldSetActive()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.Deactivate();
        user.Activate();

        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_ShouldSetInactive()
    {
        var user = new ApplicationUser("u", "e@e.com", "Name", DateTime.UtcNow, Gender.Male);
        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
