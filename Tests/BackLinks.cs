using UI.Models;
using UI.Pages;

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
    public void Construct_querystring_from_array_property_with_one_item()
    {
        var model = new SearchModel { Subjects = new[] { "first" } };
        var result = model.ToRouteData();
        result.Should().ContainKey("Subjects").WhoseValue.Should().Be("first");
    }

    [Fact]
    public void Construct_querystring_from_array_property_with_multiple_items()
    {
        var model = new SearchModel
        {
            KeyStages = new[]
            {
                KeyStage.KeyStage1, KeyStage.KeyStage2, KeyStage.KeyStage3
            }
        };

        var result = model.ToRouteData();

        result.Should().ContainKey("KeyStages")
            .WhoseValue.Should().Be("KeyStage1&KeyStages=KeyStage2&KeyStages=KeyStage3");
    }

    [Fact]
    public void Construct_querystring_from_array_property_with_null_item()
    {
        var model = new SearchModel { Subjects = new string[] { null! } };
        var result = model.ToRouteData();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Construct_querystring_from_non_null_search_model_properties()
    {
        var model = new SearchModel
        { 
            Postcode = "AB1 2CD",
            KeyStages = new[] { KeyStage.KeyStage1 },
            Subjects = null,
        };

        var result = model.ToRouteData();

        result.Should().BeEquivalentTo(new Dictionary<string, string>
        {
            { "Postcode", "AB1 2CD" },
            { "KeyStages", "KeyStage1" },
        });
    }

    [Fact]
    public void Construct_querystring_from_tutor_type()
    {
        var model = new SearchModel { TuitionType = TuitionType.Online };
        var result = model.ToRouteData();
        result.Should().ContainKey("TuitionType").WhoseValue.Should().Be("Online");
    }
}