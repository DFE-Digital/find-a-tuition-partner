using UI.Models;
using UI.Pages.FindATuitionPartner;

namespace Tests;

public class BackLinks
{
    [Fact]
    public void Construct_querystring_from_string_property()
    {
        var model = new SearchModel { Postcode = "AB1 2CD" };
        var result = model.ToRouteData();
        result.Should().ContainKey("Postcode").WhoseValue.Should().Be("AB1 2CD");
    }

    [Fact]
    public void Construct_querystring_from_array_property()
    {
        var model = new SearchModel { Subjects = new[] { "first" } };
        var result = model.ToRouteData();
        result.Should().ContainKey("Subjects").WhoseValue.Should().Be("first");
    }

    [Fact]
    public void Construct_querystring_from_non_null_search_model_properties()
    {
        var model = new SearchModel
        { 
            Postcode = "AB1 2CD",
            KeyStages = new[] { KeyStage.KeyStage1, KeyStage.KeyStage3 },
            Subjects = null,
        };

        var result = model.ToRouteData();

        result.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "Postcode", "AB1 2CD" },
            { "KeyStages", "KeyStage1&KeyStages=KeyStage3" },
        });
    }
}