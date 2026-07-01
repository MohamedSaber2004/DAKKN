using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class SystemSettingTests
{
    [Fact]
    public void Constructor_ShouldInitializeKeyValuePair()
    {
        var setting = new SystemSetting
        {
            Key = "SiteName",
            Value = "DAKKN",
            Description = "The name of the site"
        };

        setting.Id.Should().NotBeEmpty();
        setting.Key.Should().Be("SiteName");
        setting.Value.Should().Be("DAKKN");
        setting.Description.Should().Be("The name of the site");
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var setting = new SystemSetting();
        setting.Key.Should().BeEmpty();
        setting.Value.Should().BeEmpty();
        setting.Description.Should().BeNull();
    }

    [Fact]
    public void Value_ShouldBeUpdatable()
    {
        var setting = new SystemSetting { Key = "MaxItems", Value = "10" };
        setting.Value = "25";
        setting.Value.Should().Be("25");
    }
}
