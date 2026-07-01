using DAKKN.Domain.Entities;

namespace DAKKN.Tests.Tests.Domain;

public class LandingPageSettingTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var setting = new LandingPageSetting
        {
            Key = "hero_banner",
            Value = "{\"title\":\"Welcome\",\"subtitle\":\"Shop now\"}",
            Description = "Hero banner JSON configuration"
        };

        setting.Id.Should().NotBeEmpty();
        setting.Key.Should().Be("hero_banner");
        setting.Value.Should().Be("{\"title\":\"Welcome\",\"subtitle\":\"Shop now\"}");
        setting.Description.Should().Be("Hero banner JSON configuration");
    }

    [Fact]
    public void Value_ShouldStoreJsonString()
    {
        var json = "{\"slides\":[{\"image\":\"banner1.jpg\",\"link\":\"/sale\"}]}";
        var setting = new LandingPageSetting { Key = "slider", Value = json };

        setting.Value.Should().Be(json);
    }

    [Fact]
    public void DefaultValues_ShouldBeEmpty()
    {
        var setting = new LandingPageSetting();
        setting.Key.Should().BeEmpty();
        setting.Value.Should().BeEmpty();
        setting.Description.Should().BeNull();
    }
}
