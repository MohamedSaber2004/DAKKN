using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class UserSettingsTests
{
    [Fact]
    public void Constructor_ShouldUseDefaults()
    {
        var settings = new UserSettings
        {
            UserId = Guid.NewGuid()
        };

        settings.Id.Should().NotBeEmpty();
        settings.UserId.Should().NotBeEmpty();
        settings.Language.Should().Be("ar");
        settings.Theme.Should().Be("light");
        settings.PrimaryColor.Should().Be("#3B82F6");
        settings.IsDarkMode.Should().BeFalse();
        settings.LayoutMode.Should().Be("default");
    }

    [Fact]
    public void Language_ShouldDefaultToArabic()
    {
        var settings = new UserSettings();
        settings.Language.Should().Be("ar");
    }

    [Fact]
    public void Theme_ShouldDefaultToLight()
    {
        var settings = new UserSettings();
        settings.Theme.Should().Be("light");
    }

    [Fact]
    public void IsDarkMode_ShouldDefaultToFalse()
    {
        var settings = new UserSettings();
        settings.IsDarkMode.Should().BeFalse();
    }

    [Fact]
    public void Properties_ShouldBeMutable()
    {
        var settings = new UserSettings();
        settings.Language = "en";
        settings.Theme = "dark";
        settings.PrimaryColor = "#000000";
        settings.IsDarkMode = true;
        settings.LayoutMode = "compact";

        settings.Language.Should().Be("en");
        settings.Theme.Should().Be("dark");
        settings.PrimaryColor.Should().Be("#000000");
        settings.IsDarkMode.Should().BeTrue();
        settings.LayoutMode.Should().Be("compact");
    }
}
